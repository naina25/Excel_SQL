namespace ExcelSql.Models
{
    public class SimilarTableData
    {
        public bool isSimilar { get; set; }
        public string? tableName { get; set; }
        public int count { get; set; }
        public List<string>? colsList = new List<string> { };
    }
}
