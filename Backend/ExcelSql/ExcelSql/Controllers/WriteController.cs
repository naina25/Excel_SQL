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
using ExcelSql.Data;
using ExcelSql.Services;

namespace ExcelSql.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class WriteController : ControllerBase
    {
        //private readonly IConfiguration _configuration;
        //private readonly string excelPath;
        //private readonly string dbConnection;

        //public WriteController(IConfiguration configuration)
        //{
        //    _configuration = configuration;
        //    excelPath = _configuration.GetSection("ExcelPath").Value.ToString();
        //    dbConnection = _configuration.GetConnectionString("MyConnectionString");
        //}

        private readonly IExcelSQLService _excelSQLService;

        public WriteController(IExcelSQLService excelSQLService)
        {
            _excelSQLService= excelSQLService;
        }

        [HttpGet]
        [Route("sheets")]
        public IActionResult GetSheet()
        {
            return Ok(_excelSQLService.GetSheetsNames());
        }

        [HttpGet]
        [Route("sheets/{sheetName}")]
        public IActionResult GetSheetData(string sheetName)
        {
            return Ok(JsonConvert.SerializeObject(_excelSQLService.GetSheetData(sheetName)));
        }

        [HttpGet]
        [Route("sqltables")]
        public IActionResult GetTablesName()
        {
            return Ok(JsonConvert.SerializeObject(_excelSQLService.GetSQLTables()));
        }

        [HttpPut]
        [Route("sheets/{sheetName}/edit")]
        public IActionResult EditSheet(string sheetName, [FromBody] string jsonData)
        {
           
        }
    }
}
