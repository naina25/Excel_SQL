using Microsoft.Office.Interop.Excel;

namespace ExcelSql.Services
{
    public interface IExcelSQLService
    {
        List<string> GetSheetsNames();
        string GetTableData(string tableName);
        System.Data.DataTable GetSQLTables();
        string GetTableColumns(string tableName);
        string GetDistinctVals(string tableName, string colName);
        bool EditSheet(string sheetName, string jsonData);
        string GetSortedData(string tableName, string column, string order);
        string GetChartVals(string tableName, string firstCol, string secondCol, string selectedVal);
        string GetSearchData(string tableName, string searchQuery);
        string GetBarChartVals(string tableName, string firstCol, string secondCol, string[] selectedValArr);
    }
}
