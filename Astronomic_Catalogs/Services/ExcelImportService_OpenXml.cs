using Astronomic_Catalogs.Data;
using Astronomic_Catalogs.Models;
using Astronomic_Catalogs.Services.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;
using System.Globalization;


namespace Astronomic_Catalogs.Services;

public class ExcelImportService_OpenXml : IExcelImport
{
    private readonly ApplicationDbContext _context;
    private readonly string _filePath;
    private readonly ILogger<ExcelImportService_OpenXml> _logger;
    private int rowNumber = 0;

    public ExcelImportService_OpenXml(ApplicationDbContext context, ILogger<ExcelImportService_OpenXml> logger)
    {
        _context = context;
        _filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Excel", "PS_2025.03.13_23.08.05 - Converted – Clear.xlsx");
        _logger = logger;
    }

    public async Task ImportDataAsync()
    {
        if (!File.Exists(_filePath))
        {
            throw new FileNotFoundException("Excel file not found", _filePath);
        }

        using var document = SpreadsheetDocument.Open(_filePath, false);
        var sheet = document.WorkbookPart.Workbook.Sheets.GetFirstChild<Sheet>();
        if (sheet == null) throw new Exception("No sheets found in the Excel file.");

        var worksheetPart = (WorksheetPart)document.WorkbookPart.GetPartById(sheet.Id!);
        var rows = worksheetPart.Worksheet.GetFirstChild<SheetData>()?.Elements<Row>();
        if (rows == null) throw new Exception("No rows found in the Excel sheet.");

        var headerRow = rows.FirstOrDefault();

        var planets = new List<NASAExoplanetCatalog>();
        bool isHeaderSkipped = false;
        var sharedStrings = document.WorkbookPart.SharedStringTablePart?.SharedStringTable;
        int countAll = 0;
        int countInsert = 1;
        const int batchSize = 300;

        _logger.LogDebug($"START AT: {DateTime.Now}"); 
        foreach (var row in rows)
        {
            if (!isHeaderSkipped)
            {
                isHeaderSkipped = true;
                continue;
            }

            var planet = MapRowToModel<NASAExoplanetCatalog>(row, sharedStrings);
            planets.Add(planet);

            if (planets.Count >= batchSize)
            {
                _context.PlanetsCatalog.AddRange(planets);
            await _context.SaveChangesAsync();
            countAll += batchSize;
            _logger.LogDebug($"{countInsert} Count of saved rows is: {countAll}. Last saving occured at: {DateTime.Now}");
                countInsert += 1;
            planets.Clear(); 
            }
        }

        if (planets.Count > 0)
        {
            _context.PlanetsCatalog.AddRange(planets);
            await _context.SaveChangesAsync();
            _logger.LogDebug($"{countInsert} Count of saved rows is: {countAll + planets.Count}.");
            _logger.LogDebug($"FINISH AT: {DateTime.Now}");
        }
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
                        _logger.LogTrace(
                            $"Property {property.Name} with vaule {value} from cell that has №{cellIndex} FOR {rowNumber} ROW was seted.");
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
        int a = 2;
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

}
