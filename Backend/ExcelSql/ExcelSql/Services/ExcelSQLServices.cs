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

        public string GetTableData(string tableName)
        {
            return _dataLayer.GetTableData(tableName);
        }

        public System.Data.DataTable GetSQLTables()
        {
            return _dataLayer.GetSqlTables();
        }

        public string EditSheet(string sheetName, string jsonData)
        {
            jsonData = jsonData.Replace("\\", "");
            Dictionary<string, string>? dictObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
            return _dataLayer.EditSheet(sheetName, dictObj);
        }

        public string GetSortedData(string tableName, string column, string order)
        {
            order = order.ToUpper();
            if (order != "ASC" && order != "DESC")
                order = "ASC";
            return _dataLayer.GetSortedData(tableName, column, order);
        }

        public string GetTableColumns(string tableName)
        {
            return _dataLayer.GetTableColumns(tableName);
        }

        public string GetDistinctVals(string tableName, string colName)
        {
            return _dataLayer.GetDistinctEntries(tableName, colName);
        }

        public string GetChartVals(string tableName, string firstCol, string secondCol, string selectedVal)
        {
            return _dataLayer.GetChartVals(tableName, firstCol, secondCol, selectedVal);
        }

        public string GetSearchData(string tableName, string searchQuery)
        {
            return _dataLayer.GetSearchedData(tableName, searchQuery);
        }

        public string GetBarChartVals(string tableName, string firstCol, string secondCol, string[] selectedValArr)
        {
            return _dataLayer.GetBarChartVals(tableName, firstCol, secondCol, selectedValArr);
        }
    }
}
