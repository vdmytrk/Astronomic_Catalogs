using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Infrastructure.LogingIfrastructure;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using System.Globalization;


namespace Astronomic_Catalogs.Services;

public class ExcelImportService_OpenXml : IExcelImport
{
    private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
    private readonly string _filePath;
    private readonly ILogger<ExcelImportService_OpenXml> _logger;
    private readonly IHubContext<ProgressHub> _hub;
    private readonly IImportCancellationService _importCancellationService;
    private readonly DatabaseInitializer _databaseInitializer;
    private int rowNumber = 0;

    public ExcelImportService_OpenXml(
        IDbContextFactory<ApplicationDbContext> contextFactory,
        ILogger<ExcelImportService_OpenXml> logger,
        IHubContext<ProgressHub> hub,
        IImportCancellationService importCancellationService,
        DatabaseInitializer databaseInitializer)
    {
        _contextFactory = contextFactory;
        _filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Excel", "PS_2025.03.13_23.08.05 - Converted – Clear.xlsx");
        _logger = logger;
        _hub = hub;
        _importCancellationService = importCancellationService;
        _databaseInitializer = databaseInitializer;
    }


    public async Task ImportDataAsync(string jobId, CancellationToken cancellationToken)
    {
        if (!File.Exists(_filePath))
            throw new FileNotFoundException("Excel file not found", _filePath);

        const int rowsPerTask = 300; // It should be less than 1% of the total number of rows to allow for a smooth display of execution progress (so that the change can be shown for each percentage of work completed).

        using var document = SpreadsheetDocument.Open(_filePath, false);
        var sheet = document.WorkbookPart!.Workbook.Sheets!.GetFirstChild<Sheet>() ?? throw new Exception("No sheets found in the Excel file.");
        var worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id!);
        var sharedStrings = document.WorkbookPart.SharedStringTablePart?.SharedStringTable;
        var rows = worksheetPart.Worksheet.GetFirstChild<SheetData>()?.Elements<Row>().ToList() ?? [];

        rows = rows.Skip(1).ToList();

        int totalRows = rows.Count;
        int partitionCount = (int)Math.Ceiling((double)totalRows / rowsPerTask);
        int progressUpdateInterval = Math.Max(1, totalRows / 100);
        int processed = 0;
        int savedRows = 0;

        _logger.LogInformation($"[IMPORT START] Kyiv Time: {FileLogService.GetKyivTime()} | Total rows: {totalRows}");

        await TruncateTableAsync();

        var tasks = new List<Task>();
        var semaphore = new SemaphoreSlim(4);

        for (int i = 0; i < partitionCount; i++)
        {
            var taskIndex = i;
            var subset = rows.Skip(i * rowsPerTask).Take(rowsPerTask).ToList();

            await semaphore.WaitAsync(cancellationToken);

            var task = Task.Run(async () =>
            {
                try
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    _logger.LogInformation($"[Task with index: {taskIndex}] START at {FileLogService.GetKyivTime()}");
                    var localList = new List<NASAExoplanetCatalog>();

                    foreach (var row in subset)
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        var planet = MapRowToModel<NASAExoplanetCatalog>(row, sharedStrings);
                        localList.Add(planet);

                        int localProcessed = Interlocked.Increment(ref processed);
                        if (localProcessed % progressUpdateInterval == 0)
                            await _hub.Clients.Group(jobId).SendAsync("ReceiveProgress", (int)((double)localProcessed / totalRows * 100), cancellationToken);
                    }

                    await using var context = await _contextFactory.CreateDbContextAsync(cancellationToken);
                    context.PlanetsCatalog.AddRange(localList);
                    await context.SaveChangesAsync(cancellationToken);

                    Interlocked.Add(ref savedRows, localList.Count);
                    _logger.LogInformation($"Saved {savedRows} rows by task {taskIndex} at {FileLogService.GetKyivTime()}");
                }
                finally
                {
                    semaphore.Release();
                }
            }, cancellationToken);

            tasks.Add(task);
        }

        try
        {
            await Task.WhenAll(tasks);
            await _hub.Clients.Group(jobId).SendAsync("ReceiveProgress", 100, cancellationToken);
            await FillTablesAsync();
        }
        catch (OperationCanceledException)
        {
            _logger.LogWarning("Import cancelled.");
        }
        finally
        {
            _importCancellationService.Remove(jobId);
        }

        _logger.LogInformation($"[IMPORT FINISH] Kyiv Time: {FileLogService.GetKyivTime()} | Total Saved: {savedRows}");
    }


    public T MapRowToModel<T>(Row row, SharedStringTable? sharedStrings) where T : new()
    {
        var model = new T();
        var properties = typeof(T).GetProperties().ToArray();
        var cells = row.Elements<Cell>().ToList();
        int cellIndex = 0;

        rowNumber += 1;

        for (int i = 0; i < properties.Length; i++)
        {
            if (cellIndex < cells.Count)
            {
                var cell = cells[cellIndex];
                int columnIndex = GetColumnIndex(cell);

                if (columnIndex > i)
                {
                    // Skip missing cells (Excel does not save empty ones)
                    i = columnIndex - 1;
                    continue;
                }

                var property = properties[i];
                var cellValue = GetCellValue(cell, property.PropertyType, sharedStrings);

                if (property.Name == "RowId")
                    property.SetValue(model, null);
                else
                    try
                    {
                        object? value = property.PropertyType switch
                        {
                            Type t when t == typeof(DateTime) || t == typeof(DateTime?) => TryParseDate(cellValue, out var dt) ? dt : null,
                            Type t when t == typeof(string) => cellValue ?? string.Empty,
                            Type t when t == typeof(double) || t == typeof(double?) => double.TryParse(cellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out double doubleValue) ? doubleValue : null,
                            Type t when t == typeof(int) || t == typeof(int?) => int.TryParse(cellValue, out int intValue) ? intValue : null,
                            Type t when t == typeof(bool) || t == typeof(bool?) => bool.TryParse(cellValue, out bool boolValue) ? boolValue : null,
                            Type t when t == typeof(float) || t == typeof(float?) => float.TryParse(cellValue, NumberStyles.Any, CultureInfo.InvariantCulture, out float floatValue) ? floatValue : null,
                            _ => null
                        };
                        property.SetValue(model, value);
                        _logger.LogDebug($"Property {property.Name} with vaule {value} from cell that has №{cellIndex} FOR {rowNumber} ROW was seted.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error filling in the field {property.Name}: {ex.Message}");
                    }

                cellIndex++;
            }
        }

        return model;
    }

    private static bool TryParseDate(string? input, out DateTime result)
    {
        if (string.IsNullOrWhiteSpace(input))
        {
            result = DateTime.MinValue;
            return false;
        }
        string[] formats = { "yyyy-MM", "dd.MM.yyyy", "yyyy-MM-dd", "dd-MM-yyyy", "dd/MM/yyyy" };
        bool TryParseResult = DateTime.TryParseExact(input, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out result);
        return TryParseResult;
    }


    // To get column index from cell address. Conversion A->1, B->2, ..., Z->26, AA->27 ect.
    private int GetColumnIndex(Cell cell)
    {
        string reference = cell.CellReference?.Value ?? string.Empty;
        string column = new string(reference.TakeWhile(char.IsLetter).ToArray());

        int columnIndex = 0;
        foreach (char c in column)
        {
            columnIndex = (columnIndex * 26) + (c - 'A' + 1);
        }

        return columnIndex - 1;
    }

    public string GetCellValue(Cell cell, Type? propertyType, SharedStringTable? sharedStrings)
    {
        if (cell.CellValue == null) return string.Empty;

        string value = cell.CellValue.Text;

        if (cell.DataType != null && cell.DataType.Value == CellValues.SharedString && sharedStrings != null)
        {
            if (int.TryParse(value, out int index) && index >= 0 && index < sharedStrings.ChildElements.Count)
            {
                return sharedStrings.ChildElements[index].InnerText;
            }
        }

        if ((propertyType == typeof(int) || propertyType == typeof(float) || propertyType == typeof(double))
            && (cell.DataType == null || cell.DataType.Value == CellValues.Number))
        {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
            {
                return numericValue.ToString(CultureInfo.InvariantCulture);
            }
        }

        if (cell.DataType == null || cell.DataType.Value == CellValues.Number)
        {
            if (double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out double numericValue))
            {
                DateTime baseDate = new DateTime(1899, 12, 30);
                return baseDate.AddDays(numericValue).ToString("dd.MM.yyyy");
            }
        }

        return value;
    }

    private async Task TruncateTableAsync()
    {
        using var context = _contextFactory.CreateDbContext();
        await context.Database.ExecuteSqlRawAsync("TRUNCATE TABLE [NASAExoplanetCatalog]");
        _logger.LogInformation("Table NASAExoplanetCatalog cleared before import.");
    }

    private async Task FillTablesAsync()
    {
        await _databaseInitializer.ExecuteStoredProcedureAsync("FillNASAExoplanetCatalogLastUpdate");
        await _databaseInitializer.ExecuteStoredProcedureAsync("FillNASAExoplanetCatalogUniquePlanets");
        await _databaseInitializer.ExecuteStoredProcedureAsync("CalculationPlanetarySystemData"); 
    }

}
