using Microsoft.Office.Interop.Excel;

namespace ExcelSql.Services
{
    public interface IExcelSQLService
    {
        public List<string> GetSheetsNames();
        public System.Data.DataTable GetSheetData(string sheetName);
        public System.Data.DataTable GetSQLTables();
        public System.Data.DataTable GetTableColumns(string tableName);
        public void EditSheet(string sheetName, string jsonData);

        public System.Data.DataTable GetSortedData(string tableName, string column, string order);

    }
}
