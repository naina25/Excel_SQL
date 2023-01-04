using Microsoft.Office.Interop.Excel;

namespace ExcelSql.Services
{
    public interface IExcelSQLService
    {
        public List<string> GetSheetsNames();
        public System.Data.DataTable GetSheetData(string sheetName);
        public System.Data.DataTable GetSQLTables();
        public System.Data.DataTable GetTableColumns(string tableName);
        public System.Data.DataTable GetDistinctVals(string tableName, string colName);
        public void EditSheet(string sheetName, string jsonData);

        public System.Data.DataTable GetSortedData(string tableName, string column, string order);
        public System.Data.DataTable GetChartVals(string tableName, string firstCol, string secondCol, string selectedVal);

    }
}
