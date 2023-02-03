using ExcelSql.Data;
using ExcelSql.Models;
using ExcelSql.Services;
using Moq;
using System;
using Xunit;

namespace ExcelSqlXUnitTest.Services
{
    public class ValidationServiceTests
    {
        private MockRepository mockRepository;

        private Mock<IDaLayer> mockDaLayer;

        public ValidationServiceTests()
        {
            this.mockRepository = new MockRepository(MockBehavior.Strict);

            this.mockDaLayer = this.mockRepository.Create<IDaLayer>();
        }

        private ValidationService CreateService()
        {
            return new ValidationService(
                this.mockDaLayer.Object);
        }

        private ErrorModel InvalidTableError(string tableName)
        {
            var errorObj = new ErrorModel() { statusCode = 400, errorMsg = $"Table - {tableName} not found."};
            return errorObj;
        }

        private ErrorModel InvalidColError(string tableName, string colName)
        {
            var errorObj = new ErrorModel() { statusCode = 400, errorMsg = $"Column - {colName} not found in the table - {tableName}." };
            return errorObj;
        }

        private List<ErrorModel> InvalidPieErrorList(string tableName, string firstCol, string secondCol, string val, string[] failingParams)
        {
            List<ErrorModel> errorList = new();
            if (failingParams.Contains("table"))
            {
                errorList.Add(InvalidTableError(tableName));
            }
            else
            {
                if (failingParams.Contains("firstCol"))
                    errorList.Add(InvalidColError(tableName, firstCol));
                else if (failingParams.Contains("val"))
                    errorList.Add(new ErrorModel() { statusCode = 400, errorMsg = $"Value - {val} not found in the column - {firstCol}." });
                if (failingParams.Contains("secondCol"))
                    errorList.Add(InvalidColError(tableName, secondCol));

            }
            return errorList;
        }

        [Fact]
        public void ValidateTable_ValidCall()
        {
            // Arrange
            var service = this.CreateService();
            string tableName = "Sheet Data";
            var expectedObj = new ErrorModel();
            mockDaLayer.Setup(x => x.IsTablePresent(tableName)).Returns(true);

            // Act
            var result = service.ValidateTable(
                tableName);
            var actualErrorObj = result as ErrorModel;

            // Assert
            Assert.Equal(expectedObj.errorMsg, actualErrorObj.errorMsg);
            Assert.Equal(expectedObj.statusCode, actualErrorObj.statusCode);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void ValidateTable_InvalidCall()
        {
            // Arrange
            var service = this.CreateService();
            string tableName = "Sheet Datas";
            var expectedObj = InvalidTableError(tableName);
            mockDaLayer.Setup(x => x.IsTablePresent(tableName)).Returns(false);

            // Act
            var result = service.ValidateTable(
                tableName);
            var actualErrorObj = result as ErrorModel;

            // Assert
            Assert.Equal(expectedObj.statusCode, actualErrorObj.statusCode);
            Assert.Equal(expectedObj.errorMsg, actualErrorObj.errorMsg);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void ValidateColumn_ValidCall()
        {
            // Arrange
            var service = this.CreateService();
            string tableName = "Sheet Data";
            string colName = "Name";
            var expectedObj = new ErrorModel();
            mockDaLayer.Setup(x => x.IsTablePresent(tableName)).Returns(true);
            mockDaLayer.Setup(x => x.IsColumnPresent(tableName, colName)).Returns(true);

            // Act
            var result = service.ValidateColumn(
                tableName,
                colName);
            var actualErrorObj = result as ErrorModel;

            // Assert
            Assert.Equal(expectedObj.statusCode, actualErrorObj.statusCode);
            Assert.Equal(expectedObj.errorMsg, actualErrorObj.errorMsg);
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData("Sheet Data", "Name", "table")]
        [InlineData("Sheet Data", "Name", "column")]
        public void ValidateColumn_InvalidCall(string tableName, string colName, string failingParam)
        {
            // Arrange
            var service = this.CreateService();
            var expectedObj = new ErrorModel();
            if(failingParam == "table")
                expectedObj = InvalidTableError(tableName);
            mockDaLayer.Setup(x => x.IsTablePresent(tableName)).Returns(failingParam != "table");
            if (failingParam == "column")
            {
                expectedObj = InvalidColError(tableName, colName);
                mockDaLayer.Setup(x => x.IsColumnPresent(tableName, colName)).Returns(false);
            }
            // Act
            var result = service.ValidateColumn(
                tableName,
                colName);
            var actualErrorObj = result as ErrorModel;

            // Assert
            Assert.Equal(expectedObj.statusCode, actualErrorObj.statusCode);
            Assert.Equal(expectedObj.errorMsg, actualErrorObj.errorMsg);
            this.mockRepository.VerifyAll();
        }

        [Fact]
        public void ValidatePieChartReq_ValidCall()
        {
            // Arrange
            var service = this.CreateService();
            string tableName = "Sheet Data";
            string firstCol = "Name";
            string secondCol = "Age";
            string val = "Mohit";
            List<ErrorModel> expectedErrorList = new();
            mockDaLayer.Setup(x => x.IsTablePresent(tableName)).Returns(true);
            mockDaLayer.Setup(x => x.IsColumnPresent(tableName, It.IsAny<string>())).Returns(true);
            mockDaLayer.Setup(x => x.IsValuePresent(tableName, firstCol, val)).Returns(true);
            
            // Act
            var result = service.ValidatePieChartReq(
                tableName,
                firstCol,
                secondCol,
                val);
            var actualErrorList = result as List<ErrorModel>;

            // Assert
            Assert.Equal(expectedErrorList.Count, actualErrorList.Count);
            for(int i = 0; i < expectedErrorList.Count; i++)
            {
                Assert.Equal(expectedErrorList[i].errorMsg, actualErrorList[i].errorMsg);
                Assert.Equal(expectedErrorList[i].statusCode, actualErrorList[i].statusCode);
            }
            this.mockRepository.VerifyAll();
        }

        [Theory]
        [InlineData(new object[] { "Sheet Data", "Name", "Age", "Mohit", new string[] { "table" } })]
        [InlineData(new object[] { "Sheet Data", "Name", "Age", "Mohit", new string[] { "table", "secondCol" } })]
        [InlineData(new object[] { "Sheet Data", "Name", "Age", "Mohit", new string[] { "secondCol" } })]
        [InlineData(new object[] { "Sheet Data", "Name", "Age", "Mohit", new string[] { "firstCol", "secondCol" } })]
        [InlineData(new object[] { "Sheet Data", "Name", "Age", "Mohit", new string[] { "val", "secondCol" } })]
        public void ValidatePieChartReq_InvalidCall(string tableName, string firstCol, string secondCol, string val, string[] failingParams)
        {
            // Arrange
            var service = this.CreateService();
            List<ErrorModel> expectedErrorList = InvalidPieErrorList(tableName, firstCol, secondCol, val, failingParams);
            if (failingParams.Contains("table"))
                mockDaLayer.Setup(x => x.IsTablePresent(tableName)).Returns(false);
            else
            {
                mockDaLayer.Setup(x => x.IsTablePresent(tableName)).Returns(true);
                if (failingParams.Contains("firstCol"))
                {
                    mockDaLayer.Setup(x => x.IsColumnPresent(tableName, firstCol)).Returns(false);
                }
                else if (failingParams.Contains("val"))
                {
                    mockDaLayer.Setup(x => x.IsColumnPresent(tableName, firstCol)).Returns(true);
                    mockDaLayer.Setup(x => x.IsValuePresent(tableName, firstCol, val)).Returns(false);
                }
                else
                {
                    mockDaLayer.Setup(x => x.IsColumnPresent(tableName, firstCol)).Returns(true);
                    mockDaLayer.Setup(x => x.IsValuePresent(tableName, firstCol, val)).Returns(true);
                }
                if (failingParams.Contains("secondCol"))
                {
                    mockDaLayer.Setup(x => x.IsColumnPresent(tableName, secondCol)).Returns(false);
                }
                else
                    mockDaLayer.Setup(x => x.IsColumnPresent(tableName, secondCol)).Returns(true);
            }

            // Act
            var result = service.ValidatePieChartReq(
                tableName,
                firstCol,
                secondCol,
                val);
            var actualErrorList = result as List<ErrorModel>;

            // Assert
            Assert.Equal(expectedErrorList.Count, actualErrorList.Count);
            for(int i = 0; i < expectedErrorList.Count; i++)
            {
                Assert.Equal(expectedErrorList[i].errorMsg, actualErrorList[i].errorMsg);
                Assert.Equal(expectedErrorList[i].statusCode, actualErrorList[i].statusCode);
            }
            this.mockRepository.VerifyAll();
        }
    }
}
