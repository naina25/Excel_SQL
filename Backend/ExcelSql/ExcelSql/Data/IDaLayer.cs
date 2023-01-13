using ExcelSql.Models;
using Microsoft.Office.Interop.Excel;

namespace ExcelSql.Data
{
    public interface IDaLayer
    {
        public List<string> GetSheets();
        public List<List<string>> GetSheetData(Workbook _workbook, Worksheet ws);
        public List<List<string>> GetUniqueData(List<List<string>> sheetData, string tableName);
        public void CreateAndInsert(Workbook _workbook, Worksheet ws);
        public void InsertData(string tableName, List<List<string>> sheetData);
        public void AlterTable(string tableName, List<string> colsList);
        public string GetTableData(string tableName);
        public System.Data.DataTable GetSqlTables();
        public string GetTableColumns(string tableName);
        public string GetDistinctEntries(string tableName, string colName);
        public string EditSheet(string sheetName, Dictionary<string, string> dictObj);
        public string GetSortedData(string tableName, string column, string order);
        public string GetChartVals(string tableName, string firstCol, string secondCol, string selectedVal);
        public bool IsTablePresent(string tableName);
        public SimilarTableData IsTableSimilar(Workbook _workbook, Worksheet ws);
        public bool IsColumnPresent(string tableName, string colName);
        public bool IsValuePresent(string tableName, string colName, string val);

        public string GetSearchedData(string tableName, string searchQuery);

        public string GetBarChartVals(string tableName, string firstCol, string secondCol, string[] selectedValArr);

    }
}
