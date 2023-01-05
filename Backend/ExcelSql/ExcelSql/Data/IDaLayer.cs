using Microsoft.Office.Interop.Excel;

namespace ExcelSql.Data
{
    public interface IDaLayer
    {
        public List<string> GetSheets();
        public void CreateAndInsert(Workbook _workbook, Worksheet ws);
        public string GetSheetData(string sheetName);
        public System.Data.DataTable GetSqlTables();
        public string GetTableColumns(string tableName);
        public string GetDistinctEntries(string tableName, string colName);
        public string EditSheet(string sheetName, Dictionary<string, string> dictObj);
        public string GetSortedData(string tableName, string column, string order);
        public string GetChartVals(string tableName, string firstCol, string secondCol, string selectedVal);
        public bool IsTablePresent(string tableName);
        public bool IsColumnPresent(string tableName, string colName);
        public bool IsValuePresent(string tableName, string colName, string val);

    }
}
