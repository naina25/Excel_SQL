using ExcelSql.Models;

namespace ExcelSql.Services
{
    public interface IValidationService
    {
        ErrorModel ValidateTable(string tableName);
        ErrorModel ValidateColumn(string tableName, string colName);
        List<ErrorModel> ValidatePieChartReq(string tableName, string firstCol, string secondCol, string val);
        List<ErrorModel> ValidateBarChartReq(string tableName, string firstCol, string secondCol, string[] selectedValArr);
    }
}
