﻿using Microsoft.Office.Interop.Excel;

namespace ExcelSql.Data
{
    public interface IDaLayer
    {
        public List<string> GetSheets();
        public void CreateAndInsert(Workbook _workbook, Worksheet ws, string sqlDataSource);
        public System.Data.DataTable GetSheetData(string sheetName);
        public System.Data.DataTable GetSqlTables();
        public void EditSheet(string sheetName, string jsonData, Dictionary<string, string> dictObj);
        public System.Data.DataTable GetSortedData(string tableName, string column, string order);

    }
}
