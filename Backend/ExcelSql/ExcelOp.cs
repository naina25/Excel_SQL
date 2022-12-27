using Microsoft.Data.SqlClient;
using Microsoft.Office.Interop.Excel;

namespace ExcelSql
{
    public class ExcelOp
    {
        public List<string> IsTablePresent(string excelPath, string dbConnection)
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
                string query = $"SELECT CASE WHEN OBJECT_ID('dbo.{ws.Name}', 'U') IS NOT NULL THEN 'Found' ELSE 'Not Found' END";
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
                if (table.Rows[0][0].ToString() == "Found")
                {
                    Console.WriteLine("Yes");
                }
                else
                {
                    CreateAndInsert(_workbook, ws, sqlDataSource);
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

        private static void CreateAndInsert(Workbook _workbook, Worksheet ws, string sqlDataSource)
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
                    if (range.Cells[1, i] != null && range.Cells[i, j].Value2 != null)
                        rowData.Add(range.Cells[i, j].Value2.ToString());
                    else
                        rowData.Add("");
                }
                sheetData.Add(rowData);
            }

            string createQuery = $"CREATE TABLE {ws.Name} (ID int identity, ";
            for (int i = 0; i < sheetData[0].Count; i++)
            {
                createQuery += sheetData[0][i] + " nvarchar(50)";
                if (i + 1 != sheetData[0].Count)
                    createQuery += ", ";
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
                string insQuery = $"INSERT INTO dbo.{ws.Name} values (";
                for (int j = 0; j < sheetData[i].Count; j++)
                {
                    insQuery += "'" + sheetData[i][j] + "'";
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

    }
}
