using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
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
                sheetNames.Add(ws.Name);
                //string query = $"SELECT CASE WHEN OBJECT_ID('dbo.{ws.Name}', 'U') IS NOT NULL THEN 'Found' ELSE 'Not Found' END";
                //System.Data.DataTable table = new System.Data.DataTable();
                //string sqlDataSource = dbConnection;
                //SqlDataReader myReader;
                //using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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
                //if (table.Rows[0][0].ToString() == "Not Found")
                //{
                //    CreateAndInsert(_workbook, ws, sqlDataSource);
                //}
                if (!IsTablePresent(tableName: ws.Name))
                    CreateAndInsert(_workbook, ws);

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

        public void CreateAndInsert(Workbook _workbook, Worksheet ws)
        {
            Worksheet sheet = _workbook.Sheets[ws.Name];
            Microsoft.Office.Interop.Excel.Range range = sheet.UsedRange;
            string sqlDataSource = dbConnection;
            int rowCount = range.Rows.Count;
            int colCount = range.Columns.Count;
            List<List<string>> sheetData = new List<List<string>>();
            for (int i = 1; i <= rowCount; i++)
            {
                List<string> rowData = new List<string>();
                for (int j = 1; j <= colCount; j++)
                {
                    if (range.Cells[1, i] != null && range.Cells[i, j].Value2 != null)
                        rowData.Add(range.Cells[i, j].Value.ToString());
                    else
                        rowData.Add("");
                }
                sheetData.Add(rowData);
            }


            string createQuery = $"CREATE TABLE [{ws.Name}] (ID int identity, [";
            for (int i = 0; i < sheetData[0].Count; i++)
            {
                createQuery += sheetData[0][i] + "] nvarchar(max)";
                if (i + 1 != sheetData[0].Count)
                    createQuery += ", [";
            }
            createQuery += ");";



            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
            {
                myCon.Open();
                using (SqlCommand myCommand = new SqlCommand(createQuery, myCon))
                {
                    myCommand.ExecuteReader();
                    myCon.Close();
                }
            }


            for (int i = 1; i < sheetData.Count; i++)
            {
                string insQuery = $"INSERT INTO [{ws.Name}] values (";
                for (int j = 0; j < sheetData[i].Count; j++)
                {
                    insQuery += "'" + sheetData[i][j].Replace("'", "''").Replace(" 00:00:00", "") + "'";
                    if (j != sheetData[i].Count - 1)
                    {
                        insQuery += ", ";
                    }
                }
                insQuery += ");";


                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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

        public string GetSheetData(string sheetName)
        {
            string readQuery = $"SELECT * FROM [{sheetName}]";
            System.Data.DataTable table = new System.Data.DataTable();
            string sqlDataSource = dbConnection;
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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
            string sqlDataSource = dbConnection;
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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
            string sqlDataSource = dbConnection;
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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

                string sqlDataSource = dbConnection;
                using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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
            string sqlDataSource = dbConnection;
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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
            string sqlDataSource = dbConnection;
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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
            string sqlDataSource = dbConnection;
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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
            string sqlDataSource = dbConnection;
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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

        public bool IsColumnPresent(string tableName, string colName)
        {
            string query = $"SELECT CASE WHEN COL_LENGTH('{tableName}','{colName}') IS NOT NULL " +
                $"THEN 'Found' ELSE 'Not Found' END;";
            System.Data.DataTable table = new System.Data.DataTable();
            string sqlDataSource = dbConnection;
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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
            string sqlDataSource = dbConnection;
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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
            string sqlDataSource = dbConnection;
            SqlDataReader myReader;
            using (SqlConnection myCon = new SqlConnection(sqlDataSource))
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
    }
}
