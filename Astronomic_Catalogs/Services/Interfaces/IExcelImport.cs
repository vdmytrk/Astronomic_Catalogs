using DocumentFormat.OpenXml.Spreadsheet;
using ExcelDataReader;

namespace Astronomic_Catalogs.Services.Interfaces;

public interface IExcelImport
{
    Task ImportDataAsync(string jobId, CancellationToken cancellationToken);
    T MapRowToModel<T>(Row row, SharedStringTable? sharedStrings) where T : new();
    string GetCellValue(Cell cell, Type? propertyType, SharedStringTable? sharedStrings);
}
