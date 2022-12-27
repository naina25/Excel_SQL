using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Office.Interop.Excel;
using System.Data;
using System.Configuration;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using System.Data.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.Json;

namespace ExcelSql.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WriteController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly string excelPath;
        private readonly string dbConnection;

        public WriteController(IConfiguration configuration)
        {
            _configuration = configuration;
            excelPath = _configuration.GetSection("ExcelPath").Value.ToString();
            dbConnection = _configuration.GetConnectionString("MyConnectionString");
        }

        [HttpGet]
        [Route("sheets")]
        public IActionResult GetSheet()
        {
            ExcelOp excelObj = new ExcelOp();
            return Ok(excelObj.IsTablePresent(excelPath, dbConnection));
        }

        [HttpGet]
        [Route("sheets/{sheetName}")]
        public IActionResult GetSheetData(string sheetName)
        {
            string readQuery = $"SELECT * FROM dbo.{sheetName}";
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
            return Ok(JsonConvert.SerializeObject(table));
        }

        [HttpGet]
        [Route("sqlsheets")]
        public IActionResult GetSqlSheets()
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
            return Ok(JsonConvert.SerializeObject(table));
        }
        [HttpPut]
        [Route("sheets/{sheetName}/edit")]
        public IActionResult EditSheet(string sheetName, [FromBody] string jsonData)
        {
            jsonData = jsonData.Replace("\\", "");
            Console.WriteLine(jsonData);
            Dictionary<string, string> dictObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData); ;

            string updateQuery = $"UPDATE dbo.{sheetName} SET ";

            for (int i = 1; i < dictObj.Count; i++)
            {

                updateQuery += $"{dictObj.ElementAt(i).Key}='{dictObj.ElementAt(i).Value}'";
                if (i != dictObj.Count - 1)
                    updateQuery += ", ";
            }
            updateQuery += $" WHERE {dictObj.ElementAt(0).Key}={dictObj.ElementAt(0).Value};";

            Console.WriteLine(updateQuery);

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

            return Ok();
        }
    }
}
