using ExcelSql.Models;
using Microsoft.Office.Interop.Excel;

namespace ExcelSql.Data
{
    public interface IDaLayer
    {
        List<string> GetSheets();
        string GetTableData(string tableName);
        System.Data.DataTable GetSqlTables();
        string GetTableColumns(string tableName);
        string GetDistinctEntries(string tableName, string colName);
        bool EditSheet(string sheetName, Dictionary<string, string> dictObj);
        string GetSortedData(string tableName, string column, string order);
        string GetChartVals(string tableName, string firstCol, string secondCol, string selectedVal);
        bool IsTablePresent(string tableName);
        bool IsColumnPresent(string tableName, string colName);
        bool IsValuePresent(string tableName, string colName, string val);
        string GetSearchedData(string tableName, string searchQuery);
        string GetBarChartVals(string tableName, string firstCol, string secondCol, string[] selectedValArr);
    }
}
