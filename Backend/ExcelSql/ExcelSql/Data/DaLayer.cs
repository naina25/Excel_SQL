using ExcelSql.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Data.Common;

namespace ExcelSql.Data
{
    public class DaLayer:IDaLayer
    {
        private readonly IConfiguration _configuration;
        private readonly string excelPath;
        private readonly string dbConnection;

        public DaLayer(IConfiguration configuration)
        {
            _configuration = configuration;
            excelPath = _configuration.GetSection("ExcelPath").Value.ToString();
            dbConnection = _configuration.GetConnectionString("MyConnectionString");
        }
        public List<string> GetSheets()
        {
            Application xlApp = new();
            Workbooks workbooks;
            Workbook _workbook;
            workbooks = xlApp.Workbooks;


            _workbook = workbooks.Open(excelPath, ReadOnly: true, Notify: false);
            List<string> sheetNames = new();
            foreach (Worksheet ws in _workbook.Sheets)
            {
                if (!IsTablePresent(tableName: ws.Name))
                {
                    SimilarTableData similarTableObj = IsTableSimilar(_workbook, ws);
                    if (similarTableObj.isSimilar == false)     // no distinct cols or Distinct cols greater than 5
                    {
                        CreateAndInsert(_workbook, ws);
                        if (sheetNames.Contains(ws.Name) == false)
                            sheetNames.Add(ws.Name);
                    }
                    else
                    {
                        if (similarTableObj.count != 0)     // Distinct cols between 0 to 5
                        {
                            AlterTable(similarTableObj.tableName, similarTableObj.colsList);
                        }
                        List<List<string>> sheetData = GetSheetData(_workbook, ws);     // Getting data from excel sheet
                        sheetData = GetUniqueData(sheetData, similarTableObj.tableName);        // Removing data already present
                        InsertData(similarTableObj.tableName, sheetData);
                        if (sheetNames.Contains(similarTableObj.tableName) == false)
                            sheetNames.Add(similarTableObj.tableName);
                    }
                }
                else
                {
                    if (sheetNames.Contains(ws.Name) == false)
                        sheetNames.Add(ws.Name);
                }

            }
            _workbook.Close(true, excelPath, null);
            workbooks.Close();
            xlApp.Quit();
            _workbook = null;
            workbooks = null;
            xlApp = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();
            //GC.WaitForFullGCComplete();
            GC.WaitForPendingFinalizers();
            return sheetNames;

        }

        private List<List<string>> GetSheetData(Workbook _workbook, Worksheet ws)
        {
            Worksheet sheet = _workbook.Sheets[ws.Name];
            Microsoft.Office.Interop.Excel.Range range = sheet.UsedRange;
            var rowCount = range.Rows.Count;
            var colCount = range.Columns.Count;
            List<List<string>> sheetData = new();
            for (int i = 1; i <= rowCount; i++)
            {
                List<string> rowData = new();
                for (int j = 1; j <= colCount; j++)
                {
                    if (range.Cells[i, j] != null && range.Cells[i, j].Value2 != null)
                        rowData.Add(range.Cells[i, j].Value.ToString());
                    else
                        rowData.Add("");
                }
                sheetData.Add(rowData);
            }
            return sheetData;
        }

        private List<List<string>> GetUniqueData(List<List<string>> sheetData, string tableName)
        {
            List<List<string>> uniqueData = new()
            {
                sheetData[0]
            };
            for (int i = 1; i < sheetData.Count; i++)
            {
                var validateDataQuery = $"SELECT CASE WHEN EXISTS(SELECT 1 FROM [{tableName}] where [";
                for (int j = 0; j < sheetData[i].Count; j++)
                {
                    validateDataQuery += sheetData[0][j] + $"] = '{sheetData[i][j]}'";
                    if (j + 1 != sheetData[i].Count)
                        validateDataQuery += "AND [";
                }
                validateDataQuery += ") THEN 'Found' ELSE 'Not Found' END;";


                System.Data.DataTable table = new();
                SqlDataReader myReader;
                using (SqlConnection myCon = new(dbConnection))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new(validateDataQuery, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
                if(table.Rows[0][0].ToString() == "Not Found")
                {
                    uniqueData.Add(sheetData[i]);
                }
            }

            return uniqueData;
        }

        private void CreateAndInsert(Workbook _workbook, Worksheet ws)
        {
            List<List<string>> sheetData = GetSheetData(_workbook, ws);


            var createQuery = $"CREATE TABLE [{ws.Name}] (ID int identity, [";
            for (int i = 0; i < sheetData[0].Count; i++)
            {
                createQuery += sheetData[0][i] + "] nvarchar(max)";
                if (i + 1 != sheetData[0].Count)
                    createQuery += ", [";
            }
            createQuery += ");";


            using (SqlConnection myCon = new(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(createQuery, myCon))
                {
                    myCommand.ExecuteReader();
                    myCon.Close();
                }
            }

            InsertData(ws.Name, sheetData);
        }

        private void InsertData(string tableName, List<List<string>> sheetData)
        {
            for (int i = 1; i < sheetData.Count; i++)
            {
                var insQuery = $"INSERT INTO [{tableName}] values (";
                for (int j = 0; j < sheetData[i].Count; j++)
                {
                    insQuery += "'" + sheetData[i][j].Replace("'", "''").Replace(" 00:00:00", "") + "'";
                    if (j != sheetData[i].Count - 1)
                    {
                        insQuery += ", ";
                    }
                }
                insQuery += ");";


                using (SqlConnection myCon = new(dbConnection))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new(insQuery, myCon))
                    {
                        myCommand.ExecuteReader();
                        myCon.Close();
                    }
                }
            }
        }

        private void AlterTable(string tableName, List<string> colsList)
        {

            var alterQuery = $"ALTER TABLE [{tableName}] ADD ";
            for (int i = 0; i < colsList.Count; i++)
            {
                alterQuery += "[" + colsList[i] + "] nvarchar(max)";
                if (i + 1 != colsList.Count)
                    alterQuery += ",";
            }
            alterQuery += ";";

            using (SqlConnection myCon = new(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(alterQuery, myCon))
                {
                    myCommand.ExecuteReader();
                    myCon.Close();
                }
            }

        }

        public string GetTableData(string tableName)
        {
            var readQuery = $"SELECT * FROM [{tableName}]";
            System.Data.DataTable table = new();
            SqlDataReader myReader;
            using (SqlConnection myCon = new(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(readQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return JsonConvert.SerializeObject(table);
        }

        public System.Data.DataTable GetSqlTables()
        {
            var readQuery = $"SELECT TABLE_NAME as 'Table' FROM INFORMATION_SCHEMA.TABLES;";
            System.Data.DataTable table = new();
            SqlDataReader myReader;
            using (SqlConnection myCon = new(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(readQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return table;
        }

        public string GetTableColumns(string tableName)
        {
            tableName = tableName.Replace("'", "''");
            var getColQuery = $"SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('{tableName}')";
            System.Data.DataTable table = new();
            SqlDataReader myReader;
            using (SqlConnection myCon = new(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(getColQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return JsonConvert.SerializeObject(table);
        }

        public bool EditSheet(string sheetName, Dictionary<string, string> dictObj)
        {
            var updateQuery = $"UPDATE [{sheetName}] SET ";
            string key;
            string? value;

            for (int i = 1; i < dictObj.Count; i++)
            {
                key = dictObj.ElementAt(i).Key.ToString();
                if (dictObj.ElementAt(i).Value != null)
                {
                    value = dictObj.ElementAt(i).Value.ToString().Replace("'", "''");
                }
                else
                {
                    value = dictObj.ElementAt(i).Value;
                }
                updateQuery += $"[{key}]='{value}'";
                if (i != dictObj.Count - 1)
                    updateQuery += ", ";
            }
            key = dictObj.ElementAt(0).Key.ToString();
            if (dictObj.ElementAt(0).Value != null)
            {
                value = dictObj.ElementAt(0).Value.ToString().Replace("'", "''");
            }
            else
            {
                value = dictObj.ElementAt(0).Value;
            }
            updateQuery += $" WHERE [{key}]={value};";
            if (IsValuePresent(sheetName, key, value))
            {
                using (SqlConnection myCon = new(dbConnection))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new(updateQuery, myCon))
                    {
                        myCommand.ExecuteReader();
                        myCon.Close();
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetSortedData(string tableName, string column, string order)
        {
            var readQuery = $"Select * from [{tableName}] order by [{column}] {order}";
            System.Data.DataTable data = new();
            SqlDataReader myReader;
            using (SqlConnection myCon = new(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(readQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    data.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return JsonConvert.SerializeObject(data);
        }

        public string GetDistinctEntries(string tableName, string colName)
        {
            var getDistValQuery = $"SELECT DISTINCT [{colName}] FROM [{tableName}]";
            System.Data.DataTable table = new();
            SqlDataReader myReader;
            using (SqlConnection myCon = new(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(getDistValQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return JsonConvert.SerializeObject(table);
        }

        public string GetChartVals(string tableName, string firstCol, string secondCol, string selectedVal)
        {
            var getGroupedValsQuery = $"SELECT [{secondCol}], COUNT(*) AS ValCount " +
                $"FROM [{tableName}] WHERE [{firstCol}] = '{selectedVal}' GROUP BY [{secondCol}]";
            System.Data.DataTable table = new();
            SqlDataReader myReader;
            using (SqlConnection myCon = new(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(getGroupedValsQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return JsonConvert.SerializeObject(table);
        }

        public bool IsTablePresent(string tableName)
        {
            tableName = tableName.Replace("'", "''");
            var query = $"SELECT CASE WHEN OBJECT_ID('{tableName}', 'U') IS NOT NULL " +
                $"THEN 'Found' ELSE 'Not Found' END";
            System.Data.DataTable table = new();
            SqlDataReader myReader;
            using (SqlConnection myCon = new(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return (table.Rows[0][0].ToString() == "Found");
        }

        private SimilarTableData IsTableSimilar(Workbook _workbook, Worksheet ws)
        {
            SimilarTableData similarTableObj = new(); 

            Worksheet sheet = _workbook.Sheets[ws.Name];
            Microsoft.Office.Interop.Excel.Range range = sheet.UsedRange;
            var colCount = range.Columns.Count;
            List<string> sheetCols = new();
            for (int j = 1; j <= colCount; j++)
            {
                if (range.Cells[1, j] != null && range.Cells[1, j].Value2 != null)
                    sheetCols.Add(range.Cells[1, j].Value.ToString());
                else
                    sheetCols.Add("");
            }

            System.Data.DataTable allTables = GetSqlTables();
            List<string> tables = new();
            for (int i = 0; i < allTables.Rows.Count; i++)
            {
                tables.Add(allTables.Rows[i][0].ToString());
            }

            foreach (var table in tables)
            {
                SimilarTableData tempObj = new();
                var count = 0;
                List<string> columnsList = new();
                System.Data.DataTable? columnsObjList = JsonConvert.DeserializeObject<System.Data.DataTable>(GetTableColumns(table));
                for (int i = 0; i < columnsObjList.Rows.Count; i++)
                {
                    columnsList.Add(columnsObjList.Rows[i][0].ToString());
                }
                for (int j = 0; j < sheetCols.Count; j++)
                {
                    if (columnsList.Contains(sheetCols[j]) == false)
                    {
                        tempObj.colsList.Add(sheetCols[j]);
                        count++;
                    };
                }
                Console.WriteLine($"count {count}");
                tempObj.count = count;
                tempObj.tableName = table;
                if (count == 0)
                {
                    tempObj.isSimilar = true;
                    return tempObj;
                }
                else if (tempObj.count > 0 && tempObj.count < 5)
                {
                    if(similarTableObj.count == 0 || similarTableObj.count > tempObj.count)
                    {
                        similarTableObj.count = tempObj.count;
                        similarTableObj.tableName = table;
                        similarTableObj.isSimilar = true;
                        similarTableObj.colsList = tempObj.colsList;
                    }

                }

            }
            return similarTableObj;
        }

        public bool IsColumnPresent(string tableName, string colName)
        {
            tableName = tableName.Replace("'", "''");
            colName = colName.Replace("'", "''");
            var query = $"SELECT CASE WHEN COL_LENGTH('{tableName}','{colName}') IS NOT NULL " +
                $"THEN 'Found' ELSE 'Not Found' END;";
            System.Data.DataTable table = new();
            SqlDataReader myReader;
            using (SqlConnection myCon = new(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return (table.Rows[0][0].ToString() == "Found");
        }

        public bool IsValuePresent(string tableName, string colName, string val)
        {
            val = val.Replace("'", "''");
            var query = $"SELECT CASE WHEN EXISTS(SELECT 1 FROM [{tableName}] " +
                $"where [{colName}]='{val}') THEN 'Found' ELSE 'Not Found' END;";
            System.Data.DataTable table = new();
            SqlDataReader myReader;
            using (SqlConnection myCon = new(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return (table.Rows[0][0].ToString() == "Found");
        }

        public string GetSearchedData(string tableName, string searchQuery)
        {
            searchQuery = searchQuery.Replace("'", "''");
            List<string> columnList = new();
            foreach(var obj in JsonConvert.DeserializeObject<List<JObject>>(GetTableColumns(tableName))){
                columnList.Add(obj["name"].ToString());
            };

            var dataQuery = $"SELECT * FROM [{tableName}] where (";

            for(int i = 0; i < columnList.Count; i++)
            {
                dataQuery += $" [{columnList[i]}] LIKE '%{searchQuery}%'";
                if(i != columnList.Count - 1)
                {
                    dataQuery += " OR ";
                }
                else
                {
                    dataQuery += ")";
                }
            }

            System.Data.DataTable table = new();
            SqlDataReader myReader;
            using (SqlConnection myCon = new(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new(dataQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return JsonConvert.SerializeObject(table);
        }

        public string GetBarChartVals(string tableName, string firstCol, string secondCol, string[] selectedValArr)
        {
            List<string> dataList = new();
            foreach (var val in selectedValArr)
            {
                var getGroupedValsQuery = $"SELECT [{secondCol}], COUNT(*) AS ValCount " +
                $"FROM [{tableName}] WHERE [{firstCol}] = '{val}' GROUP BY [{secondCol}]";
                System.Data.DataTable table = new();
                SqlDataReader myReader;
                using (SqlConnection myCon = new(dbConnection))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new(getGroupedValsQuery, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
                dataList.Add(JsonConvert.SerializeObject(table));
            }
            
            return JsonConvert.SerializeObject(dataList) ;
        }
    }
}
