using ExcelSql.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;

namespace ExcelSql.Services
{
    public class ExcelSQLServices : IExcelSQLService
    {
        private readonly IDaLayer _dataLayer;

        public ExcelSQLServices(IDaLayer dataLayer)
        {
            _dataLayer = dataLayer;
        }

        public List<string> GetSheetsNames()
        {
            return _dataLayer.GetSheets();
        }

        public System.Data.DataTable GetSheetData(string sheetName)
        {
            return _dataLayer.GetSheetData(sheetName);
        }

        public System.Data.DataTable GetSQLTables()
        {
            return _dataLayer.GetSqlTables();
        }

        public void EditSheet(string sheetName, string jsonData)
        {
            jsonData = jsonData.Replace("\\", "");
            Dictionary<string, string>? dictObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
            _dataLayer.EditSheet(sheetName, jsonData, dictObj);
        }

        public System.Data.DataTable GetSortedData(string tableName, string column, string order)
        {
            return _dataLayer.GetSortedData(tableName, column, order);
        }
    }
}
