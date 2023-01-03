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
    public class ExcelDataController : ControllerBase
    {
        private readonly IExcelSQLService _excelSQLService;

        public ExcelDataController(IExcelSQLService excelSQLService)
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

        [HttpGet]
        [Route("sqltables/{tableName}")]
        public IActionResult GetColumnNames(string tableName)
        {
            return Ok(JsonConvert.SerializeObject(_excelSQLService.GetTableColumns(tableName)));
        }

        [HttpPut]
        [Route("sheets/{sheetName}/edit")]
        public IActionResult EditSheet(string sheetName, [FromBody] string jsonData)
        {
            _excelSQLService.EditSheet(sheetName, jsonData);
            return Ok();
        }

        [HttpGet]
        [Route("sheets/sort/{tableName}")]
        public IActionResult GetSortedData(string tableName, string column, string order)
        {
            return Ok(JsonConvert.SerializeObject(_excelSQLService.GetSortedData(tableName, column, order)));
        }
    }
}
