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
            Application xlApp = new Application();
            Workbooks workbooks;
            Workbook _workbook;
            workbooks = xlApp.Workbooks;


            _workbook = workbooks.Open(excelPath, ReadOnly: true, Notify: false);
            List<string> sheetNames = new List<string>();
            foreach (Worksheet ws in _workbook.Sheets)
            {
                if (!IsTablePresent(tableName: ws.Name))
                {
                    SimilarTableData similarTableObj = IsTableSimilar(_workbook, ws);
                    if (similarTableObj.isSimilar == false)
                    {
                        CreateAndInsert(_workbook, ws);
                        sheetNames.Add(ws.Name);
                    }
                    else
                    {
                        if (similarTableObj.count != 0)
                        {
                            AlterTable(similarTableObj.tableName, similarTableObj.colsList);
                        }
                        List<List<string>> sheetData = GetSheetData(_workbook, ws);
                        sheetData = GetUniqueData(sheetData, similarTableObj.tableName);
                        foreach (var row in sheetData)
                        {
                            foreach (var item in row)
                            {
                                Console.WriteLine(item);
                            }
                        }
                        InsertData(similarTableObj.tableName, sheetData);
                        sheetNames.Add(similarTableObj.tableName);
                    }
                }
                else
                {
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

        public List<List<string>> GetSheetData(Workbook _workbook, Worksheet ws)
        {
            Worksheet sheet = _workbook.Sheets[ws.Name];
            Microsoft.Office.Interop.Excel.Range range = sheet.UsedRange;
            int rowCount = range.Rows.Count;
            int colCount = range.Columns.Count;
            List<List<string>> sheetData = new List<List<string>>();
            for (int i = 1; i <= rowCount; i++)
            {
                List<string> rowData = new List<string>();
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

        public List<List<string>> GetUniqueData(List<List<string>> sheetData, string tableName)
        {
            List<List<string>> uniqueData= new List<List<string>>();
            uniqueData.Add(sheetData[0]);
            for (int i = 1; i < sheetData.Count; i++)
            {
                string validateDataQuery = $"SELECT CASE WHEN EXISTS(SELECT 1 FROM [{tableName}] where [";
                for (int j = 0; j < sheetData[i].Count; j++)
                {
                    validateDataQuery += sheetData[0][j] + $"] = '{sheetData[i][j].ToString()}'";
                    if (j + 1 != sheetData[i].Count)
                        validateDataQuery += "AND [";
                }
                validateDataQuery += ") THEN 'Found' ELSE 'Not Found' END;";


                System.Data.DataTable table = new System.Data.DataTable();
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(dbConnection))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(validateDataQuery, myCon))
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




            //$"{colName}]='{val}') THEN 'Found' ELSE 'Not Found' END;";
            //System.Data.DataTable table = new System.Data.DataTable();
            //SqlDataReader myReader;
            //using (SqlConnection myCon = new SqlConnection(dbConnection))
            //{
            //    myCon.Open();
            //    using (SqlCommand myCommand = new SqlCommand(query, myCon))
            //    {
            //        myReader = myCommand.ExecuteReader();
            //        table.Load(myReader);
            //        myReader.Close();
            //        myCon.Close();
            //    }
            //}
            //return (table.Rows[0][0].ToString() == "Found");
        }

        public void CreateAndInsert(Workbook _workbook, Worksheet ws)
        {
            List<List<string>> sheetData = GetSheetData(_workbook, ws);


            string createQuery = $"CREATE TABLE [{ws.Name}] (ID int identity, [";
            for (int i = 0; i < sheetData[0].Count; i++)
            {
                createQuery += sheetData[0][i] + "] nvarchar(max)";
                if (i + 1 != sheetData[0].Count)
                    createQuery += ", [";
            }
            createQuery += ");";


            using (SqlConnection myCon = new SqlConnection(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(createQuery, myCon))
                {
                    myCommand.ExecuteReader();
                    myCon.Close();
                }
            }

            InsertData(ws.Name, sheetData);
        }

        public void InsertData(string tableName, List<List<string>> sheetData)
        {
            for (int i = 1; i < sheetData.Count; i++)
            {
                string insQuery = $"INSERT INTO [{tableName}] values (";
                for (int j = 0; j < sheetData[i].Count; j++)
                {
                    insQuery += "'" + sheetData[i][j].Replace("'", "''").Replace(" 00:00:00", "") + "'";
                    if (j != sheetData[i].Count - 1)
                    {
                        insQuery += ", ";
                    }
                }
                insQuery += ");";


                using (SqlConnection myCon = new SqlConnection(dbConnection))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(insQuery, myCon))
                    {
                        myCommand.ExecuteReader();
                        myCon.Close();
                    }
                }
            }
        }

        public void AlterTable(string tableName, List<string> colsList)
        {

            string alterQuery = $"ALTER TABLE [{tableName}] ADD ";
            for (int i = 0; i < colsList.Count; i++)
            {
                alterQuery += "[" + colsList[i] + "] nvarchar(max)";
                if (i + 1 != colsList.Count)
                    alterQuery += ",";
            }
            alterQuery += ";";

            using (SqlConnection myCon = new SqlConnection(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(alterQuery, myCon))
                {
                    myCommand.ExecuteReader();
                    myCon.Close();
                }
            }

        }

        public string GetTableData(string tableName)
        {
            string readQuery = $"SELECT * FROM [{tableName}]";
            System.Data.DataTable table = new System.Data.DataTable();
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(readQuery, myCon))
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
            string readQuery = $"SELECT TABLE_NAME as 'Table' FROM INFORMATION_SCHEMA.TABLES;";
            System.Data.DataTable table = new System.Data.DataTable();
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(readQuery, myCon))
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
            string getColQuery = $"SELECT name FROM sys.columns WHERE object_id = OBJECT_ID('{tableName}')";
            System.Data.DataTable table = new System.Data.DataTable();
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(getColQuery, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return JsonConvert.SerializeObject(table);
        }

        public string EditSheet(string sheetName, Dictionary<string, string> dictObj)
        {
            try
            {
                string updateQuery = $"UPDATE [{sheetName}] SET ";

                for (int i = 1; i < dictObj.Count; i++)
                {

                    updateQuery += $"[{dictObj.ElementAt(i).Key}]='{dictObj.ElementAt(i).Value}'";
                    if (i != dictObj.Count - 1)
                        updateQuery += ", ";
                }
                updateQuery += $" WHERE [{dictObj.ElementAt(0).Key}]='{dictObj.ElementAt(0).Value}';";

                using (SqlConnection myCon = new SqlConnection(dbConnection))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(updateQuery, myCon))
                    {
                        myCommand.ExecuteReader();
                        myCon.Close();
                    }
                }
                return "Data Updated Successfully";
            }
            catch(Exception ex)
            {
                return $"Error occured: {ex.Message}";
            }
        }

        public string GetSortedData(string tableName, string column, string order)
        {
            string readQuery = $"Select * from [{tableName}] order by [{column}] {order}";
            System.Data.DataTable data = new System.Data.DataTable();
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(readQuery, myCon))
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
            string getDistValQuery = $"SELECT DISTINCT [{colName}] FROM [{tableName}]";
            System.Data.DataTable table = new System.Data.DataTable();
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(getDistValQuery, myCon))
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
            string getGroupedValsQuery = $"SELECT [{secondCol}], COUNT(*) AS ValCount " +
                $"FROM [{tableName}] WHERE [{firstCol}] = '{selectedVal}' GROUP BY [{secondCol}]";
            System.Data.DataTable table = new System.Data.DataTable();
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(getGroupedValsQuery, myCon))
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
            string query = $"SELECT CASE WHEN OBJECT_ID('{tableName}', 'U') IS NOT NULL " +
                $"THEN 'Found' ELSE 'Not Found' END";
            System.Data.DataTable table = new System.Data.DataTable();
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
                {
                    myReader = myCommand.ExecuteReader();
                    table.Load(myReader);
                    myReader.Close();
                    myCon.Close();
                }
            }
            return (table.Rows[0][0].ToString() == "Found");
        }

        public SimilarTableData IsTableSimilar(Workbook _workbook, Worksheet ws)
        {
            SimilarTableData similarTableObj = new SimilarTableData(); 

            Worksheet sheet = _workbook.Sheets[ws.Name];
            Microsoft.Office.Interop.Excel.Range range = sheet.UsedRange;
            int colCount = range.Columns.Count;
            List<string> sheetCols = new List<string>();
            for (int j = 1; j <= colCount; j++)
            {
                if (range.Cells[1, j] != null && range.Cells[1, j].Value2 != null)
                    sheetCols.Add(range.Cells[1, j].Value.ToString());
                else
                    sheetCols.Add("");
            }

            System.Data.DataTable allTables = GetSqlTables();
            List<string> tables = new List<string>();
            for (int i = 0; i < allTables.Rows.Count; i++)
            {
                tables.Add(allTables.Rows[i][0].ToString());
            }
            Console.WriteLine(allTables.Rows.Count);

            foreach (var table in tables)
            {
                SimilarTableData tempObj = new SimilarTableData();
                Console.WriteLine(tempObj.isSimilar);
                int count = 0;
                List<string> columnsList = new List<string>();
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
            string query = $"SELECT CASE WHEN COL_LENGTH('{tableName}','{colName}') IS NOT NULL " +
                $"THEN 'Found' ELSE 'Not Found' END;";
            System.Data.DataTable table = new System.Data.DataTable();
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
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
            string query = $"SELECT CASE WHEN EXISTS(SELECT 1 FROM [{tableName}] " +
                $"where [{colName}]='{val}') THEN 'Found' ELSE 'Not Found' END;";
            System.Data.DataTable table = new System.Data.DataTable();
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(query, myCon))
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
            List<string> columnList = new List<string>();
            foreach(var obj in JsonConvert.DeserializeObject<List<JObject>>(GetTableColumns(tableName))){
                columnList.Add(obj["name"].ToString());
            };

            string dataQuery = $"SELECT * FROM [{tableName}] where (";

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

            System.Data.DataTable table = new System.Data.DataTable();
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(dbConnection))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(dataQuery, myCon))
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
            List<string> dataList = new List<string>();
            foreach (var val in selectedValArr)
            {
                string getGroupedValsQuery = $"SELECT [{secondCol}], COUNT(*) AS ValCount " +
                $"FROM [{tableName}] WHERE [{firstCol}] = '{val}' GROUP BY [{secondCol}]";
                System.Data.DataTable table = new System.Data.DataTable();
                SqlDataReader myReader;
                using (SqlConnection myCon = new SqlConnection(dbConnection))
                {
                    myCon.Open();
                    using (SqlCommand myCommand = new SqlCommand(getGroupedValsQuery, myCon))
                    {
                        myReader = myCommand.ExecuteReader();
                        table.Load(myReader);
                        myReader.Close();
                        myCon.Close();
                    }
                }
                //Console.WriteLine(JsonConvert.SerializeObject(table));
                dataList.Add(JsonConvert.SerializeObject(table));
            }
            
            return JsonConvert.SerializeObject(dataList) ;
            
        }
    }
}
