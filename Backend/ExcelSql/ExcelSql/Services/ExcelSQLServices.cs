using ExcelSql.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Office.Interop.Excel;
using Newtonsoft.Json;

namespace ExcelSql.Services
{
    public class ExcelSQLServices : IExcelSQLService
    {
        private readonly IDaLayer _dataLayer;

        private readonly IValidationService _validationService;

        public ExcelSQLServices(IDaLayer dataLayer, IValidationService validationService)
        {
            _dataLayer = dataLayer;
            _validationService = validationService;
        }

        public List<string> GetSheetsNames()
        {
            return _dataLayer.GetSheets();
        }

        public string GetTableData(string tableName)
        {
            if (_validationService.IsTablePresent(tableName))
            {
                return _dataLayer.GetTableData(tableName);
            }
            else
            {
                return $"Table - {tableName} not found.";
            }
        }

        public System.Data.DataTable GetSQLTables()
        {
            return _dataLayer.GetSqlTables();
        }

        public string EditSheet(string sheetName, string jsonData)
        {
            if (_validationService.IsTablePresent(sheetName))
            {
                try
                {
                    jsonData = jsonData.Replace("\\", "");
                    Dictionary<string, string>? dictObj = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
                    return _dataLayer.EditSheet(sheetName, dictObj);
                }
                catch (Exception ex)
                {
                    return $"Error occured: {ex.Message}";
                }
            }
            else
            {
                return $"Table - {sheetName} not found.";
            }
        }

        public string GetSortedData(string tableName, string column, string order)
        {
            if (_validationService.IsTablePresent(tableName))
            {
                if (_validationService.IsColumnPresent(tableName, column))
                {
                    return _dataLayer.GetSortedData(tableName, column, order);
                }
                else
                {
                    return $"Column - {column} not found in the table.";
                }
            }
            else
            {
                return $"Table - {tableName} not found.";
            }
        }

        public string GetTableColumns(string tableName)
        {
            if (_validationService.IsTablePresent(tableName))
            {
                return _dataLayer.GetTableColumns(tableName);
            }
            else
            {
                return $"Table - {tableName} not found.";
            }
        }

        public string GetDistinctVals(string tableName, string colName)
        {
            if (_validationService.IsTablePresent(tableName))
            {
                if (_validationService.IsColumnPresent(tableName, colName))
                {
                    return _dataLayer.GetDistinctEntries(tableName, colName);
                }
                else
                {
                    return $"Column - {colName} not found in the table.";
                }
            }
            else
            {
                return $"Table - {tableName} not found.";
            }
        }

        public string GetChartVals(string tableName, string firstCol, string secondCol, string selectedVal)
        {
            if (_validationService.IsTablePresent(tableName))
            {
                if (_validationService.IsColumnPresent(tableName, firstCol))
                {
                    if (_validationService.IsColumnPresent(tableName, secondCol))
                    {
                        if (_validationService.IsValuePresent(tableName, firstCol, selectedVal))
                        {
                            return _dataLayer.GetChartVals(tableName, firstCol, secondCol, selectedVal);
                        }
                        else
                        {
                            return $"Entry - {selectedVal} not found in the column - {firstCol}.";
                        }
                    }
                    else
                    {
                        return $"Column - {secondCol} not found in the table.";
                    }
                }
                else
                {
                    return $"Column - {firstCol} not found in the table.";
                }
            }
            else
            {
                return $"Table - {tableName} not found.";
            }
        }

        public string GetSearchData(string tableName, string searchQuery)
        {
            if (_validationService.IsTablePresent(tableName))
            {
                return _dataLayer.GetSearchedData(tableName, searchQuery);
            }
            else
            {
                return $"Table - {tableName} not found";
            }

        }

        public string GetBarChartVals(string tableName, string firstCol, string secondCol, string[] selectedValArr)
        {
            if (_validationService.IsTablePresent(tableName))
            {
                if (_validationService.IsColumnPresent(tableName, firstCol))
                {
                    if (_validationService.IsColumnPresent(tableName, secondCol))
                    {
                        foreach (var val in selectedValArr)
                        {
                            if (!_validationService.IsValuePresent(tableName, firstCol, val))
                            {
                                return $"Entry - {val} not found in the column - {firstCol}.";
                            }
                        }

                        return _dataLayer.GetBarChartVals(tableName, firstCol, secondCol, selectedValArr);
                        
                    }
                    else
                    {
                        return $"Column - {secondCol} not found in the table.";
                    }
                }
                else
                {
                    return $"Column - {firstCol} not found in the table.";
                }
            }
            else
            {
                return $"Table - {tableName} not found.";
            }
        }
    }
}
