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
            return Ok(_excelSQLService.GetSheetsNames());
        }

        [HttpGet]
        [Route("sheets/{tableName}")]
        public IActionResult GetTableData(string tableName)
        {
            return Ok(_excelSQLService.GetTableData(tableName));
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
            return Ok(_excelSQLService.GetTableColumns(tableName));
        }

        [HttpGet]
        [Route("sqltables/{tableName}/{colName}")]
        public IActionResult GetDistinctVals(string tableName, string colName)
        {
            return Ok(_excelSQLService.GetDistinctVals(tableName, colName));
        }

        [HttpPut]
        [Route("sheets/{sheetName}/edit")]
        public IActionResult EditSheet(string sheetName, [FromBody] string jsonData)
        {
            
            return Ok(_excelSQLService.EditSheet(sheetName, jsonData));
        }

        [HttpGet]
        [Route("sheets/sort/{tableName}")]
        public IActionResult GetSortedData(string tableName, string column, string order)
        {
            return Ok(_excelSQLService.GetSortedData(tableName, column, order));
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

        [HttpGet]
        [Route("sqltables/{tableName}/Barchart")]
        public IActionResult GetBarChartVals(string tableName, string firstCol, string secondCol,[FromQuery(Name= "selectedVal[]")] string[] selectedValArr)
        {
            return Ok(JsonConvert.SerializeObject(_excelSQLService.GetBarChartVals(tableName, firstCol, secondCol, selectedValArr)));
        }
    }
}
