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

        private readonly IValidationService _validationService;

        public ExcelDataController(IExcelSQLService excelSQLService, IValidationService validationService)
        {
            _excelSQLService = excelSQLService;
            _validationService = validationService;
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
            if (!_validationService.IsTablePresent(tableName))
            {
                return BadRequest($"Table - {tableName} not found.");
            }
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
            if (!_validationService.IsTablePresent(tableName))
            {
                return BadRequest($"Table - {tableName} not found.");
            }
            return Ok(_excelSQLService.GetTableColumns(tableName));
        }

        [HttpGet]
        [Route("sqltables/{tableName}/{colName}")]
        public IActionResult GetDistinctVals(string tableName, string colName)
        {
            if (!_validationService.IsTablePresent(tableName))
            {
                return BadRequest($"Table - {tableName} not found.");
            }
            if (!_validationService.IsColumnPresent(tableName, colName))
            {
                return BadRequest($"Column - {colName} not found in the table.");
            }
            return Ok(_excelSQLService.GetDistinctVals(tableName, colName));
        }

        [HttpPut]
        [Route("sheets/{sheetName}/edit")]
        public IActionResult EditSheet(string sheetName, [FromBody] string jsonData)
        {
            if (!_validationService.IsTablePresent(sheetName))
            {
                return BadRequest($"Table - {sheetName} not found.");
            }
            try
            {
                if (_excelSQLService.EditSheet(sheetName, jsonData))
                    return Ok("Data updated successfully");
                else
                    return BadRequest($"The field you're trying to update doesn't exist in the table: {sheetName}!");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return BadRequest("Data entered by you is in an incorrect format!");
            }
        }

        [HttpGet]
        [Route("sheets/sort/{tableName}")]
        public IActionResult GetSortedData(string tableName, string column, string order)
        {
            if (!_validationService.IsTablePresent(tableName))
            {
                return BadRequest($"Table - {tableName} not found.");
            }
            if (!_validationService.IsColumnPresent(tableName, column))
            {
                return BadRequest($"Column - {column} not found in the table.");
            }
            return Ok(_excelSQLService.GetSortedData(tableName, column, order));
        }

        [HttpGet]
        [Route("sqltables/{tableName}/chart")]
        public IActionResult GetChartVals(string tableName, string firstCol, string secondCol, string selectedVal)
        {
            if (!_validationService.IsTablePresent(tableName))
            {
                return BadRequest($"Table - {tableName} not found.");
            }
            if (!_validationService.IsColumnPresent(tableName, firstCol))
            {
                return BadRequest($"Column - {firstCol} not found in the table.");
            }
            if (!_validationService.IsColumnPresent(tableName, secondCol))
            {
                return BadRequest($"Column - {secondCol} not found in the table.");
            }
            if (!_validationService.IsValuePresent(tableName, firstCol, selectedVal))
            {
                return BadRequest($"Entry - {selectedVal} not found in the column - {firstCol}.");
            }
            return Ok(_excelSQLService.GetChartVals(tableName, firstCol, secondCol, selectedVal));
        }

        [HttpGet]
        [Route("sheets/search/{tableName}")]
        public IActionResult search(string tableName, string searchQuery)
        {
            if (!_validationService.IsTablePresent(tableName))
            {
                return BadRequest($"Table - {tableName} not found");
            }
            return Ok(_excelSQLService.GetSearchData(tableName, searchQuery));
        }

        [HttpGet]
        [Route("sqltables/{tableName}/Barchart")]
        public IActionResult GetBarChartVals(string tableName, string firstCol, string secondCol, [FromQuery(Name = "selectedVal[]")] string[] selectedValArr)
        {
            if (!_validationService.IsTablePresent(tableName))
            {
                return BadRequest($"Table - {tableName} not found.");
            }
            if (!_validationService.IsColumnPresent(tableName, firstCol))
            {
                return BadRequest($"Column - {firstCol} not found in the table.");
            }
            if (!_validationService.IsColumnPresent(tableName, secondCol))
            {
                return BadRequest($"Column - {secondCol} not found in the table.");
            }
            foreach (var val in selectedValArr)
            {
                if (!_validationService.IsValuePresent(tableName, firstCol, val))
                {
                    return BadRequest($"Entry - {val} not found in the column - {firstCol}.");
                }
            }
            return Ok(_excelSQLService.GetBarChartVals(tableName, firstCol, secondCol, selectedValArr));
        }
    }
}
