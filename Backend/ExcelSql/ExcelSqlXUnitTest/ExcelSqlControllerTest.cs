using ExcelSql.Controllers;
using ExcelSql.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExcelSqlXUnitTest
{
    public class ExcelSqlControllerTest
    {
        private Mock<IExcelSQLService> _excelSQLServiceMock;
        private Mock<IValidationService> _validationServiceMock;
        private ExcelDataController? _controller;

        public ExcelSqlControllerTest()
        {
            _excelSQLServiceMock = new Mock<IExcelSQLService>();
            _validationServiceMock= new Mock<IValidationService>();
        }

        [Fact]
        private void GetTableDataTest()
        {
            var tableName = "Sheet Data";
            var expectedData = "[{ \"name\":\"ID\"},{ \"name\":\"Name\"},{ \"name\":\"Age\"},{ \"name\":\"Email\"},{ \"name\":\"Mobile\"},{ \"name\":\"Gender\"},{ \"name\":\"Salary\"},{ \"name\":\"Address\"}]";

            _excelSQLServiceMock.Setup(service => service.GetTableColumns(It.IsAny<string>()));

            _controller = new ExcelDataController(_excelSQLServiceMock.Object, _validationServiceMock.Object);
            var result = _controller.GetColumnNames(tableName);
            var obj = result as ObjectResult;
            Assert.Equal(expectedData, obj.Value);
        }
    }
}