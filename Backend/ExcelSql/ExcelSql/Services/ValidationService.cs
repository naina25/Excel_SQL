using ExcelSql.Data;

namespace ExcelSql.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IDaLayer _dataLayer;
        public ValidationService(IDaLayer dataLayer)
        {
            _dataLayer = dataLayer;
        }

        public bool IsTablePresent(string tableName)
        {
            return _dataLayer.IsTablePresent(tableName);
        }

        public bool IsColumnPresent(string tableName, string colName)
        {
            return _dataLayer.IsColumnPresent(tableName, colName);
        }

        public bool IsValuePresent(string tableName, string colName, string val)
        {
            return _dataLayer.IsValuePresent(tableName, colName, val);
        }
    }
}
