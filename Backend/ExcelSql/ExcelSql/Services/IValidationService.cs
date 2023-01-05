namespace ExcelSql.Services
{
    public interface IValidationService
    {
        public bool IsTablePresent(string tableName);
        public bool IsColumnPresent(string tableName, string colName);
        public bool IsValuePresent(string tableName, string colName, string val);
    }
}
