using Microsoft.Office.Interop.Excel;

namespace ExcelSql.Services
{
    public interface IExcelSQLService
    {
        public List<string> GetSheetsNames();
        public string GetSheetData(string sheetName);
        public System.Data.DataTable GetSQLTables();
        public string GetTableColumns(string tableName);
        public string GetDistinctVals(string tableName, string colName);
        public string EditSheet(string sheetName, string jsonData);

        public string GetSortedData(string tableName, string column, string order);
        public string GetChartVals(string tableName, string firstCol, string secondCol, string selectedVal);

        public string GetSearchData(string tableName, string searchQuery);

    }
}
