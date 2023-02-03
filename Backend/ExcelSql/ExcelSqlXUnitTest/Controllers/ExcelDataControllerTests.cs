using Castle.Components.DictionaryAdapter.Xml;
using ExcelSql.Controllers;
using ExcelSql.Models;
using ExcelSql.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Moq;
using Newtonsoft.Json;
using System;
using System.Linq;
using Xunit;

namespace ExcelSqlXUnitTest.Controllers
{
    public class ExcelDataControllerTests
    {
        private MockRepository mockRepository;

        private Mock<IExcelSQLService> mockExcelSQLService;
        private Mock<IValidationService> mockValidationService;

        public ExcelDataControllerTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockExcelSQLService = this.mockRepository.Create<IExcelSQLService>();
            this.mockValidationService = this.mockRepository.Create<IValidationService>();
        }

        private ExcelDataController CreateExcelDataController()
        {
            return new ExcelDataController(
                this.mockExcelSQLService.Object,
                this.mockValidationService.Object);
        }

        private ErrorModel GetEmptyError()
        {
            return new ErrorModel();
        }

        private List<ErrorModel> GetEmptyErrorList()
        {
            return new List<ErrorModel>();
        }

        private static List<string> GetSheetNames()
        {
            var output = new List<string>() { "Sheet Data" };
            return output;
        }

        private static System.Data.DataTable GetDummyTableNames()
        {
            var output = "[{\"Table\":\"Story Details-Oct(2022)\"}," +
                "{\"Table\":\"Non SP work-Oct\"},{\"Table\":\"Sheet Data\"}," +
                "{\"Table\":\"Table'sda\"},{\"Table\":\"Pro''ducts\"}]";

            return JsonConvert.DeserializeObject<System.Data.DataTable>(output);
        }

        private static string GetDummyTableData()
        {
            var output = "[{\"ID\":1,\"Name\":\"Ajay\",\"Age\":\"23\"," +
                "\"Email\":\"ajay@mail.com\",\"Mobile\":\"34264325\"," +
                "\"Gender\":\"M\",\"Salary\":\"\",\"Address\":\"\"}," +
                "{\"ID\":2,\"Name\":\"Atul\",\"Age\":\"26\",\"Email\":\"ajay@mail.com\"," +
                "\"Mobile\":\"354856\",\"Gender\":\"M\",\"Salary\":\"\",\"Address\":null}," +
                "{\"ID\":3,\"Name\":\"Mohit\",\"Age\":\"23\",\"Email\":\"mohit@mail.com\"," +
                "\"Mobile\":\"9238953\",\"Gender\":\"M\",\"Salary\":\"\",\"Address\":null}," +
                "{\"ID\":4,\"Name\":\"Saif\",\"Age\":\"27\",\"Email\":\"said@mail.com\"," +
                "\"Mobile\":\"895238752\",\"Gender\":\"M\",\"Salary\":\"\",\"Address\":null}," +
                "{\"ID\":5,\"Name\":\"Riya\",\"Age\":\"24\",\"Email\":\"riya@mail.com\"," +
                "\"Mobile\":\"8732957\",\"Gender\":\"F\",\"Salary\":\"20000\",\"Address\":null}," +
                "{\"ID\":6,\"Name\":\"Rohit\",\"Age\":\"26\",\"Email\":\"rohit@mail\"," +
                "\"Mobile\":\"398539\",\"Gender\":\"M\",\"Salary\":\"3000\",\"Address\":\"Noida\"}]" ;
            return output;
        }

        private static string GetDummyColumns()
        {
            var output = "[{\"name\":\"ID\"},{\"name\":\"Name\"}," +
                "{\"name\":\"Age\"},{\"name\":\"Email\"}," +
                "{\"name\":\"Mobile\"},{\"name\":\"Gender\"}," +
                "{\"name\":\"Salary\"},{\"name\":\"Address\"}]";
            return output;
        }

        private static string GetDummyDistinctVals()
        {
            var output = "[{\"Name\":\"Ajay\"},{\"Name\":\"Atul\"}," +
                "{\"Name\":\"Mohit\"},{\"Name\":\"Riya\"}," +
                "{\"Name\":\"Rohit\"},{\"Name\":\"Saif\"}]";
            return output;
        }

        private static string GetDummySortedData()
        {
            var output = "[{\"ID\":1,\"Name\":\"Ajay\",\"Age\":\"23\"," +
                "\"Email\":\"ajay@mail.com\",\"Mobile\":\"34264325\"," +
                "\"Gender\":\"M\",\"Salary\":\"\",\"Address\":\"\"}," +
                "{\"ID\":2,\"Name\":\"Atul\",\"Age\":\"26\",\"Email\":\"ajay@mail.com\"," +
                "\"Mobile\":\"354856\",\"Gender\":\"M\",\"Salary\":\"\",\"Address\":null}," +
                "{\"ID\":3,\"Name\":\"Mohit\",\"Age\":\"23\",\"Email\":\"mohit@mail.com\"," +
                "\"Mobile\":\"9238953\",\"Gender\":\"M\",\"Salary\":\"\",\"Address\":null}," +
                "{\"ID\":5,\"Name\":\"Riya\",\"Age\":\"24\",\"Email\":\"riya@mail.com\"," +
                "\"Mobile\":\"8732957\",\"Gender\":\"F\",\"Salary\":\"20000\",\"Address\":null}," +
                "{\"ID\":6,\"Name\":\"Rohit\",\"Age\":\"26\",\"Email\":\"rohit@mail\"," +
                "\"Mobile\":\"398539\",\"Gender\":\"M\",\"Salary\":\"3000\",\"Address\":\"Noida\"}," +
                "{\"ID\":4,\"Name\":\"Saif\",\"Age\":\"27\",\"Email\":\"said@mail.com\"," +
                "\"Mobile\":\"895238752\",\"Gender\":\"M\",\"Salary\":\"\",\"Address\":null}]";
            return output;
        }

        private static string GetDummyPieVals()
        {
            var output = "[{\"Age\":\"23\",\"ValCount\":1}]";
            return output;
        }

        private static string GetDummyBarVals()
        {
            var output = "[\"[{\\\"Ticket Type\\\":\\\"Bug\\\"," +
                "\\\"ValCount\\\":4},{\\\"Ticket Type\\\":\\\"Story\\\"," +
                "\\\"ValCount\\\":4},{\\\"Ticket Type\\\":\\\"Task\\\"," +
                "\\\"ValCount\\\":5}]\",\"[{\\\"Ticket Type\\\":\\\"Bug\\\"," +
                "\\\"ValCount\\\":1},{\\\"Ticket Type\\\":\\\"Story\\\"," +
                "\\\"ValCount\\\":1},{\\\"Ticket Type\\\":\\\"Task \\\"," +
                "\\\"ValCount\\\":8}]\"]";
            return output;
        }

        private static string GetDummySearchedVals()
        {
            var output = "[{\"ID\":1,\"Name\":\"Ajay\",\"Age\":\"23\"," +
                "\"Email\":\"ajay@mail.com\",\"Mobile\":\"34264325\"," +
                "\"Gender\":\"M\",\"Salary\":\"\",\"Address\":\"\"}," +
                "{\"ID\":2,\"Name\":\"Atul\",\"Age\":\"26\"," +
                "\"Email\":\"ajay@mail.com\",\"Mobile\":\"354856\"," +
                "\"Gender\":\"M\",\"Salary\":\"\",\"Address\":null}]";
            return output;
        }

        private static ErrorModel InvalidTableError(string tableName)
        {
            ErrorModel errorObj = new();
            errorObj.statusCode = 400;
            errorObj.errorMsg = $"Table - {tableName} not found.";
            return errorObj;
        }

        private static ErrorModel InvalidColumnError(string tableName, string colName, string failingParam)
        {
            ErrorModel errorObj = new();
            errorObj.statusCode = 400;
            if (failingParam == "table")
                errorObj.errorMsg = $"Table - {tableName} not found.";
            else
                errorObj.errorMsg = $"Column - {colName} not found in the table - {tableName}.";
            return errorObj;
        }

        private static List<ErrorModel> InvalidPieChartErrors(string tableName,
            string firstCol,
            string secondCol,
            string selectedVal,
            string[] failingParams)
        {

            List<ErrorModel> errList = new();
            if (failingParams.Contains("table"))
                errList.Add(new ErrorModel() { statusCode = 400, errorMsg = $"Table - {tableName} not found." });
            else
            {
                if(failingParams.Contains("firstCol"))
                {
                    errList.Add(new ErrorModel() { statusCode = 400, errorMsg = $"Column - {firstCol} not found in the table - {tableName}." });
                }
                else if(failingParams.Contains("selectedVal"))
                {
                    errList.Add(new ErrorModel() { statusCode = 400, errorMsg = $"Value - {selectedVal} not found in the column - {firstCol}." });
                }
                if(failingParams.Contains("secondCol"))
                {
                    errList.Add(new ErrorModel() { statusCode = 400, errorMsg = $"Column - {secondCol} not found in the table - {tableName}." });
                }
            }
            return errList;
        }

        private static List<ErrorModel> InvalidBarChartErrors(string tableName,
            string firstCol,
            string secondCol,
            string[] selectedValArr,
            string[] failingParams)
        {
            List<ErrorModel> errList = new();
            if (failingParams.Contains("table"))
                errList.Add(new ErrorModel() { statusCode = 400, errorMsg = $"Table - {tableName} not found." });
            else
            {
                if (failingParams.Contains("firstCol"))
                {
                    errList.Add(new ErrorModel() { statusCode = 400, errorMsg = $"Column - {firstCol} not found in the table - {tableName}." });
                }
                else
                {
                    for(int i = 0; i < selectedValArr.Length; i++)
                    {
                        if(failingParams.Contains($"selectedValArr[{i}]"))
                        {
                            errList.Add(new ErrorModel() { statusCode = 400, 
                                errorMsg = $"Value - {selectedValArr[i]} not found in the column - {firstCol}." });
                        }
                    }
                }
                if (failingParams.Contains("secondCol"))
                {
                    errList.Add(new ErrorModel() { statusCode = 400, errorMsg = $"Column - {secondCol} not found in the table - {tableName}." });
                }
            }
            return errList;
        }

        [Fact]
        public void GetSheet_ValidCall()
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            var sheets = GetSheetNames();
            mockExcelSQLService.Setup(x => x.GetSheetsNames()).Returns(GetSheetNames());

            // Act
            var result = excelDataController.GetSheet();
            var obj = result as ObjectResult;

            // Assert
            Assert.Equal(sheets, obj.Value);
            Assert.Equal(200, obj.StatusCode);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData("Sheet Data")]
        [InlineData("Sheet1")]
        public void GetTableData_ValidCall(string tableName)
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            var expectedTableData = GetDummyTableData();
            mockValidationService.Setup(x => x.ValidateTable(tableName)).Returns(GetEmptyError());
            mockExcelSQLService.Setup(x => x.GetTableData(tableName)).Returns(GetDummyTableData());

            // Act
            var result = excelDataController.GetTableData(
                tableName);
            var obj = result as ObjectResult;

            // Assert
            Assert.Equal(expectedTableData, obj.Value);
            Assert.Equal(200, obj.StatusCode);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData("Sheets Data")]
        [InlineData("Sheet31")]
        public void GetTableData_InvalidCall(string tableName)
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            var expectedErrorObj = InvalidTableError(tableName);
            mockValidationService.Setup(x => x.ValidateTable(tableName)).Returns(InvalidTableError(tableName));

            // Act
            var result = excelDataController.GetTableData(tableName);
            var obj = result as ObjectResult;
            var actualErr = obj.Value as ErrorModel;

            // Assert
            Assert.Equal(400, obj.StatusCode);
            Assert.Equal(expectedErrorObj.statusCode, actualErr.statusCode);
            Assert.Equal(expectedErrorObj.errorMsg, actualErr.errorMsg);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetTablesName_ValidCall()
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            var expectedTablesNames = JsonConvert.SerializeObject(GetDummyTableNames());
            mockExcelSQLService.Setup(x => x.GetSQLTables()).Returns(GetDummyTableNames());

            // Act
            var result = excelDataController.GetTablesName();
            var obj = result as ObjectResult;

            // Assert
            Assert.Equal(expectedTablesNames, obj.Value);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData("Sheet Data")]
        [InlineData("Sheet1")]
        public void GetColumnNames_ValidCall(string tableName)
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            var expectedTableColumns = GetDummyColumns();
            mockValidationService.Setup(x => x.ValidateTable(tableName)).Returns(GetEmptyError());
            mockExcelSQLService.Setup(x => x.GetTableColumns(tableName)).Returns(GetDummyColumns());

            // Act
            var result = excelDataController.GetColumnNames(tableName);
            var obj = result as ObjectResult;

            // Assert
            Assert.Equal(expectedTableColumns, obj.Value);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData("Sheets Data")]
        [InlineData("Sheet31")]
        public void GetColumnNames_InvalidCall(string tableName)
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            var expectedErrorObj = InvalidTableError(tableName);
            mockValidationService.Setup(x => x.ValidateTable(tableName)).Returns(InvalidTableError(tableName));

            // Act
            var result = excelDataController.GetColumnNames(tableName);
            var obj = result as ObjectResult;
            var actualErr = obj.Value as ErrorModel;

            // Assert
            Assert.Equal(400, obj.StatusCode);
            Assert.Equal(expectedErrorObj.statusCode, actualErr.statusCode);
            Assert.Equal(expectedErrorObj.errorMsg, actualErr.errorMsg);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetDistinctVals_ValidCall()
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            string tableName = "Sheet Data";
            string colName = "Name";
            var expectedVals = GetDummyDistinctVals();
            mockValidationService.Setup(x => x.ValidateColumn(tableName, colName)).Returns(GetEmptyError());
            mockExcelSQLService.Setup(x => x.GetDistinctVals(tableName, colName)).Returns(GetDummyDistinctVals());

            // Act
            var result = excelDataController.GetDistinctVals(
                tableName,
                colName);
            var obj = result as ObjectResult;

            // Assert
            Assert.Equal(expectedVals, obj.Value);
            Assert.Equal(200, obj.StatusCode);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData("Sheet Data", "Names", "table")]
        [InlineData("Sheet1", "Teams", "column")]
        public void GetDistinctVals_InvalidCall(string tableName, string colName, string failingParam)
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            var expectedErrorObj = InvalidColumnError(tableName, colName, failingParam);
            mockValidationService.Setup(x => x.ValidateColumn(tableName, colName)).Returns(InvalidColumnError(tableName, colName, failingParam));

            // Act
            var result = excelDataController.GetDistinctVals(
                tableName,
                colName);
            var obj = result as ObjectResult;
            var actualErr = obj.Value as ErrorModel;

            // Assert
            Assert.Equal(400, obj.StatusCode);
            Assert.Equal(expectedErrorObj.statusCode, actualErr.statusCode);
            Assert.Equal(expectedErrorObj.errorMsg, actualErr.errorMsg);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void EditSheet_StateUnderTest_ExpectedBehavior()
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            string sheetName = null;
            string jsonData = null;

            // Act
            var result = excelDataController.EditSheet(
                sheetName,
                jsonData);

            // Assert
            Assert.True(false);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetSortedData_ValidCall()
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            string tableName = "Sheet Data";
            string column = "Name";
            string order = "Asc";
            var expectedSortedData = GetDummySortedData();
            mockValidationService.Setup(x => x.ValidateColumn(tableName, column)).Returns(GetEmptyError());
            mockExcelSQLService.Setup(x => x.GetSortedData(tableName, column, order)).Returns(GetDummySortedData());

            // Act
            var result = excelDataController.GetSortedData(
                tableName,
                column,
                order);
            var obj = result as ObjectResult;

            // Assert
            Assert.Equal(expectedSortedData, obj.Value);
            Assert.Equal(200, obj.StatusCode);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData("Sheet Datas", "Name", "Asc", "table")]
        [InlineData("Sheet Data", "Names", "Desc", "column")]
        public void GetSortedData_InvalidCall(string tableName, string column, string order, string failingParam)
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            var expectedErrorObj = InvalidColumnError(tableName, column, failingParam);
            mockValidationService.Setup(x => x.ValidateColumn(tableName, column)).Returns(InvalidColumnError(tableName, column, failingParam));

            // Act
            var result = excelDataController.GetSortedData(
                tableName,
                column,
                order);
            var obj = result as ObjectResult;
            var actualErr = obj.Value as ErrorModel;

            // Assert
            Assert.Equal(400, obj.StatusCode);
            Assert.Equal(expectedErrorObj.statusCode, actualErr.statusCode);
            Assert.Equal(expectedErrorObj.errorMsg, actualErr.errorMsg);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetChartVals_ValidCall()
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            string tableName = "Sheet Data";
            string firstCol = "Name";
            string secondCol = "Age";
            string selectedVal = "Abc";
            var expectedPieVals = GetDummyPieVals();

            mockValidationService.Setup(x => x.ValidatePieChartReq(tableName, 
                firstCol, 
                secondCol, 
                selectedVal)).Returns(GetEmptyErrorList());

            mockExcelSQLService.Setup(x => x.GetChartVals(tableName, 
                firstCol, 
                secondCol, 
                selectedVal)).Returns(GetDummyPieVals());

            // Act
            var result = excelDataController.GetChartVals(
                tableName,
                firstCol,
                secondCol,
                selectedVal);
            var obj = result as ObjectResult;

            // Assert
            Assert.Equal(expectedPieVals, obj.Value);
            Assert.Equal(200, obj.StatusCode);
            this.mockRepository.VerifyAll();
        }


        [Theory]
        [InlineData( new object[] { "Sheet Datas", "Name", "Age", "Mohit", new string[] { "table" } } )]
        [InlineData( new object[] { "Sheet Data", "Names", "Age", "Mohit", new string[] { "firstCol" } } )]
        [InlineData( new object[] { "Sheet Data", "Names", "Ages", "Mohit", new string[] { "firstCol", "secondCol" } } )]
        [InlineData( new object[] { "Sheet Data", "Name", "Ages", "Mohita", new string[] { "secondCol", "selectedVal" } } )]
        [InlineData( new object[] { "Sheet Data", "Names", "Ages", "Mohita", new string[] { "firstCol", "secondCol", "selectedVal" } } )]
        [InlineData( new object[] { "Sheet Datas", "Names", "Ages", "Mohita", new string[] { "table", "firstCol", "secondCol", "selectedVal" } } )]
        public void GetChartVals_InvalidCall(string tableName, string firstCol, string secondCol, string selectedVal, string[] failingParams)
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            var expectedPieErr = InvalidPieChartErrors(tableName,
                firstCol,
                secondCol,
                selectedVal,
                failingParams);

            mockValidationService.Setup(x => x.ValidatePieChartReq(tableName,
                firstCol,
                secondCol,
                selectedVal)).Returns(InvalidPieChartErrors(tableName,
                firstCol,
                secondCol,
                selectedVal,
                failingParams));

            // Act
            var result = excelDataController.GetChartVals(
                tableName,
                firstCol,
                secondCol,
                selectedVal);
            var obj = result as ObjectResult;
            var actualErrList = obj.Value as List<ErrorModel>;

            // Assert
            Assert.Equal(400, obj.StatusCode);
            Assert.Equal(expectedPieErr.Count, actualErrList.Count);
            for (int i = 0; i < expectedPieErr.Count; i++)
            {
                Assert.Equal(expectedPieErr[i].statusCode, actualErrList[i].statusCode);
                Assert.Equal(expectedPieErr[i].errorMsg, actualErrList[i].errorMsg);
            }
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void search_ValidCall()
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            string tableName = "Sheet Data";
            string searchQuery = "Ajay";
            var expectedVal = GetDummySearchedVals();
            mockValidationService.Setup(x => x.ValidateTable(tableName)).Returns(GetEmptyError());
            mockExcelSQLService.Setup(x => x.GetSearchData(tableName, searchQuery)).Returns(GetDummySearchedVals());

            // Act
            var result = excelDataController.search(
                tableName,
                searchQuery);
            var obj = result as ObjectResult;

            // Assert
            Assert.Equal(200, obj.StatusCode);
            Assert.Equal(expectedVal, obj.Value);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void search_InvalidCall()
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            string tableName = "Sheets Data";
            string searchQuery = "Ajay";
            var expectedErrorObj = InvalidTableError(tableName);
            mockValidationService.Setup(x => x.ValidateTable(tableName)).Returns(InvalidTableError(tableName));

            // Act
            var result = excelDataController.search(
                tableName,
                searchQuery);
            var obj = result as ObjectResult;
            var actualErr = obj.Value as ErrorModel;

            // Assert
            Assert.Equal(400, obj.StatusCode);
            Assert.Equal(expectedErrorObj.statusCode, actualErr.statusCode);
            Assert.Equal(expectedErrorObj.errorMsg, actualErr.errorMsg);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void GetBarChartVals_ValidCall()
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            string tableName = "Story Details-Oct(2022)";
            string firstCol = "Name";
            string secondCol = "Ticket Type";
            string[] selectedValArr = { "Mayank Jindal", "Richa Sharma" };
            var expectedBarVal = GetDummyBarVals();
            mockValidationService.Setup(x => x.ValidateBarChartReq(tableName,
                firstCol,
                secondCol,
                selectedValArr)).Returns(GetEmptyErrorList());
            mockExcelSQLService.Setup(x => x.GetBarChartVals(tableName,
                firstCol,
                secondCol,
                selectedValArr)).Returns(GetDummyBarVals());

            // Act
            var result = excelDataController.GetBarChartVals(
                tableName,
                firstCol,
                secondCol,
                selectedValArr);
            var obj = result as ObjectResult;

            // Assert
            Assert.Equal(200, obj.StatusCode);
            Assert.Equal(expectedBarVal, obj.Value);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(new object[] { "Story Deetails-Oct(2022)",
            "Name", "Ticket Type",
            new string[] { "Mayank Jindal", "Richa Sharma" },
            new string[] { "table" } })]

        [InlineData(new object[] { "Story Details-Oct(2022)",
            "Names", "Ticket Type",
            new string[] { "Mayank Jindal", "Richa Sharma" },
            new string[] { "firstCol" } })]

        [InlineData(new object[] { "Story Details-Oct(2022)",
            "Names", "Ticket Types",
            new string[] { "Mayank Jindal", "Richa Sharma" },
            new string[] { "firstCol", "secondCol" } })]

        [InlineData (new object[] { "Story Details-Oct(2022)",
            "Names", "Ticket Types",
            new string[] { "Mayank Jinda", "Richa Sharma", "Mohit" },
            new string[] { "secondCol", "selectedValArr[0]", "selectedValArr[2]" } })]

        [InlineData (new object[] { "Story Details-Oct(2022)",
            "Names", "Ticket Types",
            new string[] { "Mayank Jinda", "Richa Sharma", "Mohit" },
            new string[] { "table", "firstCol", "secondCol", "selectedValArr[0]", "selectedValArr[2]" } })]

        public void GetBarChartVals_InvalidCall(string tableName,
            string firstCol,
            string secondCol,
            string[] selectedValArr,
            string[] failingParams)
        {
            // Arrange
            var excelDataController = this.CreateExcelDataController();
            var expectedBarErr = InvalidBarChartErrors(tableName,
                firstCol,
                secondCol,
                selectedValArr,
                failingParams);

            mockValidationService.Setup(x => x.ValidateBarChartReq(tableName,
                firstCol,
                secondCol,
                selectedValArr)).Returns(InvalidBarChartErrors(tableName,
                firstCol,
                secondCol,
                selectedValArr,
                failingParams));

            // Act
            var result = excelDataController.GetBarChartVals(
                tableName,
                firstCol,
                secondCol,
                selectedValArr);
            var obj = result as ObjectResult;
            var actualErrList = obj.Value as List<ErrorModel>;

            // Assert
            Assert.Equal(400, obj.StatusCode);
            Assert.Equal(expectedBarErr.Count, actualErrList.Count);
            for (int i = 0; i < expectedBarErr.Count; i++)
            {
                Assert.Equal(expectedBarErr[i].statusCode, actualErrList[i].statusCode);
                Assert.Equal(expectedBarErr[i].errorMsg, actualErrList[i].errorMsg);
            }
            this.mockRepository.VerifyAll();
        }
    }
}