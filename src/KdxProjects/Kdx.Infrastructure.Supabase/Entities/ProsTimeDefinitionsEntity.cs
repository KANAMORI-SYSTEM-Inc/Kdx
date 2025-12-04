using Kdx.Contracts.DTOs;
using Postgrest.Attributes;
using Postgrest.Models;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("ProsTimeDefinitions")]
    internal class ProsTimeDefinitionsEntity : BaseModel
    {
        [Column("OperationCategoryId")]
        public int OperationCategoryId { get; set; }

        [Column("SortOrder")]
        public int SortOrder { get; set; }

        [Column("OperationDefinitionsId")]
        public int OperationDefinitionsId { get; set; }

        [Column("Comment1")]
        public string? Comment1 { get; set; }

        [Column("Comment2")]
        public string? Comment2 { get; set; }

        public static ProsTimeDefinitionsEntity FromDto(ProsTimeDefinitions dto) => new()
        {
            OperationCategoryId = dto.OperationCategoryId,
            SortOrder = dto.SortOrder,
            OperationDefinitionsId = dto.OperationDefinitionsId,
            Comment1 = dto.Comment1,
            Comment2 = dto.Comment2
        };

        public ProsTimeDefinitions ToDto() => new()
        {
            OperationCategoryId = this.OperationCategoryId,
            SortOrder = this.SortOrder,
            OperationDefinitionsId = this.OperationDefinitionsId,
            Comment1 = this.Comment1,
            Comment2 = this.Comment2
        };
    }
}
