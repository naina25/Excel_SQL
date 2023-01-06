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
using Microsoft.EntityFrameworkCore.Metadata.Internal;

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
            Console.WriteLine("Sheet Function");
            return Ok(_excelSQLService.GetSheetsNames());   // Not needed
        }

        [HttpGet]
        [Route("sheets/{sheetName}")]
        public IActionResult GetSheetData(string sheetName)
        {
            return Ok(_excelSQLService.GetSheetData(sheetName));  //Done
        }

        [HttpGet]
        [Route("sqltables")]
        public IActionResult GetTablesName()
        {
            return Ok(JsonConvert.SerializeObject(_excelSQLService.GetSQLTables()));   // Not needed
        }

        [HttpGet]
        [Route("sqltables/{tableName}")]
        public IActionResult GetColumnNames(string tableName)
        {
            return Ok(_excelSQLService.GetTableColumns(tableName));  //Done
        }

        [HttpGet]
        [Route("sqltables/{tableName}/{colName}")]
        public IActionResult GetDistinctVals(string tableName, string colName)
        {
            return Ok(_excelSQLService.GetDistinctVals(tableName, colName));  //Done
        }

        [HttpPut]
        [Route("sheets/{sheetName}/edit")]
        public IActionResult EditSheet(string sheetName, [FromBody] string jsonData)
        {
            
            return Ok(_excelSQLService.EditSheet(sheetName, jsonData));  //Done
        }

        [HttpGet]
        [Route("sheets/sort/{tableName}")]
        public IActionResult GetSortedData(string tableName, string column, string order)
        {
            return Ok(_excelSQLService.GetSortedData(tableName, column, order));  //Done
        }

        [HttpGet]
        [Route("sqltables/{tableName}/chart")]
        public IActionResult GetChartVals(string tableName, string firstCol, string secondCol, string selectedVal)
        {
            return Ok(JsonConvert.SerializeObject(_excelSQLService.GetChartVals(tableName, firstCol, secondCol, selectedVal)));
        }

        [HttpGet]
        [Route("sheets/search/{tableName}")]
        public IActionResult search(string tableName, string searchQuery)
        {
            return Ok(_excelSQLService.GetSearchData(tableName, searchQuery));
        }
    }
}
