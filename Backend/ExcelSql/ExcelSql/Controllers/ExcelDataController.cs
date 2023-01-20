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
using Microsoft.EntityFrameworkCore.Metadata;

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
            var validateTableError = _validationService.ValidateTable(tableName);
            if (validateTableError.errorMsg != null)
            {
                return BadRequest(validateTableError);
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
            var validateTableError = _validationService.ValidateTable(tableName);
            if (validateTableError.errorMsg != null)
            {
                return BadRequest(validateTableError);
            }
            return Ok(_excelSQLService.GetTableColumns(tableName));
        }

        [HttpGet]
        [Route("sqltables/{tableName}/{colName}")]
        public IActionResult GetDistinctVals(string tableName, string colName)
        {
            var validateColError = _validationService.ValidateColumn(tableName, colName);
            if (validateColError.errorMsg != null)
            {
                return BadRequest(validateColError);
            }
            return Ok(_excelSQLService.GetDistinctVals(tableName, colName));
        }

        [HttpPut]
        [Route("sheets/{sheetName}/edit")]
        public IActionResult EditSheet(string sheetName, [FromBody] string jsonData)
        {
            var validateTableError = _validationService.ValidateTable(sheetName);
            if (validateTableError.errorMsg != null)
            {
                return BadRequest(validateTableError);
            }
            if (_excelSQLService.EditSheet(sheetName, jsonData))
                return Ok("Data updated successfully");
            else
                return BadRequest($"The field you're trying to update doesn't exist in the table: {sheetName}!");
        }

        [HttpGet]
        [Route("sheets/sort/{tableName}")]
        public IActionResult GetSortedData(string tableName, string column, string order)
        {
            var validateColError = _validationService.ValidateColumn(tableName, column);
            if (validateColError.errorMsg != null)
            {
                return BadRequest(validateColError);
            }
            return Ok(_excelSQLService.GetSortedData(tableName, column, order));
        }

        [HttpGet]
        [Route("sqltables/{tableName}/chart")]
        public IActionResult GetChartVals(string tableName, string firstCol, string secondCol, string selectedVal)
        {
            var validateValueErrList = _validationService.ValidatePieChartReq(tableName, firstCol, secondCol, selectedVal);
            if(validateValueErrList.Count != 0)
            {
                return BadRequest(validateValueErrList);
            }
            return Ok(_excelSQLService.GetChartVals(tableName, firstCol, secondCol, selectedVal));
        }

        [HttpGet]
        [Route("sheets/search/{tableName}")]
        public IActionResult search(string tableName, string searchQuery)
        {
            var validateTableError = _validationService.ValidateTable(tableName);
            if (validateTableError.errorMsg != null)
            {
                return BadRequest(validateTableError);
            }
            return Ok(_excelSQLService.GetSearchData(tableName, searchQuery));
        }

        [HttpGet]
        [Route("sqltables/{tableName}/Barchart")]
        public IActionResult GetBarChartVals(string tableName, string firstCol, string secondCol, [FromQuery(Name = "selectedVal[]")] string[] selectedValArr)
        {
            var validateBarErrList = _validationService.ValidateBarChartReq(tableName, firstCol, secondCol, selectedValArr);
            if (validateBarErrList.Count != 0)
            {
                return BadRequest(validateBarErrList);
            }
            return Ok(_excelSQLService.GetBarChartVals(tableName, firstCol, secondCol, selectedValArr));
        }
    }
}
