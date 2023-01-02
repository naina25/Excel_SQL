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

        public void EditSheet(string jsonData, string sheetName)
        {
            jsonData = jsonData.Replace("\\", "");
            Console.WriteLine(jsonData);
            Dictionary<string, string> dictObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData); ;
        }
    }
}
