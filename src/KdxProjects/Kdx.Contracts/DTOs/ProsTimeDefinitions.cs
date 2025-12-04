namespace Kdx.Contracts.DTOs
{
    public class ProsTimeDefinitions
    {
        public int OperationCategoryId { get; set; }
        public int SortOrder { get; set; }
        public int OperationDefinitionsId { get; set; } // 外部キーとして OperationDefinitions テーブルの ID を参照

        public string? Comment1 { get; set; }

        public string? Comment2 { get; set; }
    }
}
