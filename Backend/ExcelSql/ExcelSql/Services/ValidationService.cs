using ExcelSql.Data;
using ExcelSql.Models;

namespace ExcelSql.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IDaLayer _dataLayer;
        public ValidationService(IDaLayer dataLayer)
        {
            _dataLayer = dataLayer;
        }

        private bool IsTablePresent(string tableName)
        {
            return _dataLayer.IsTablePresent(tableName);
        }

        private bool IsColumnPresent(string tableName, string colName)
        {
            return _dataLayer.IsColumnPresent(tableName, colName);
        }

        private bool IsValuePresent(string tableName, string colName, string val)
        {
            return _dataLayer.IsValuePresent(tableName, colName, val);
        }

        public ErrorModel ValidateTable (string tableName)
        {
            ErrorModel errorObj = new();
            if (!IsTablePresent(tableName))
            {
                errorObj.statusCode = 400;
                errorObj.errorMsg = $"Table - {tableName} not found.";
            }
            return errorObj;
        }


        public ErrorModel ValidateColumn (string tableName, string colName)
        {
            ErrorModel errorObj = new();
            if(!IsTablePresent(tableName))
            {
                errorObj.statusCode = 400;
                errorObj.errorMsg = $"Table - {tableName} not found.";
            }
            else if(!IsColumnPresent(tableName, colName))
            {
                errorObj.statusCode = 400;
                errorObj.errorMsg = $"Column - {colName} not found in the table - {tableName}.";
            }
            return errorObj;
        }

        private ErrorModel ValidateValue (string tableName, string colName,string val)
        {
            ErrorModel errorObj = new();
            if (!IsValuePresent(tableName, colName, val))
            {
                errorObj.statusCode = 400;
                errorObj.errorMsg = $"Value - {val} not found in the column - {colName}.";
            }
            return errorObj;
        }

        public List<ErrorModel> ValidatePieChartReq(string tableName, string firstCol, string secondCol, string val)
        {
            List<ErrorModel> errorList = new();
            var validateTable = ValidateTable(tableName);
            if (validateTable.errorMsg != null)
                errorList.Add(validateTable);
            else
            {
                var validateCol = ValidateColumn(tableName, firstCol);
                if (validateCol.errorMsg != null)
                    errorList.Add(validateCol);
                else if (errorList.Count == 0)
                {
                    var validateVal = ValidateValue(tableName, firstCol, val);
                    if (validateVal.errorMsg != null)
                        errorList.Add(validateVal);
                }
                validateCol = ValidateColumn(tableName, secondCol);
                if (validateCol.errorMsg != null)
                    errorList.Add(validateCol);
            }
            return errorList;
        }

        public List<ErrorModel> ValidateBarChartReq(string tableName, string firstCol, string secondCol, string[] selectedValArr)
        {
            List<ErrorModel> errorList = new();
            var validateTable = ValidateTable(tableName);
            if (validateTable.errorMsg != null)
                errorList.Add(validateTable);
            else
            {
                var validateCol = ValidateColumn(tableName, firstCol);
                if (validateCol.errorMsg != null)
                    errorList.Add(validateCol);
                else if (errorList.Count == 0)
                {
                    foreach (var val in selectedValArr)
                    {
                        var validateVal = ValidateValue(tableName, firstCol, val);
                        if (validateVal.errorMsg != null)
                            errorList.Add(validateVal);
                    }
                }
                validateCol = ValidateColumn(tableName, secondCol);
                if (validateCol.errorMsg != null)
                    errorList.Add(validateCol);
            }
            return errorList;
        }
    }
}
