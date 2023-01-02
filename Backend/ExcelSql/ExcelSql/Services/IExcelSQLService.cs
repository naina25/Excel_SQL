using Microsoft.Office.Interop.Excel;

namespace ExcelSql.Services
{
    public interface IExcelSQLService
    {
        public List<string> GetSheetsNames();
        public System.Data.DataTable GetSheetData(string sheetName);
        public System.Data.DataTable GetSQLTables();
        public void EditSheet();

    }
}
