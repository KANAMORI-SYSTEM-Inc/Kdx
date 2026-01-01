namespace Kdx.Contracts.DTOs
{
    public class ProsTimeDefinitions
    {
        public long OperationCategoryId { get; set; }  // bigint → long
        public long SortOrder { get; set; }  // bigint → long
        public long? OperationDefinitionsId { get; set; }  // bigint null → long? (nullable)

        public string? Comment1 { get; set; }

        public string? Comment2 { get; set; }
    }
}
