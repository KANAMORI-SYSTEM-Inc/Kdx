using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("OperationCategory")]
    internal class OperationCategoryEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("CategoryName")]
        public string CategoryName { get; set; } = string.Empty;

        [Column("CategoryType")]
        public string CategoryType { get; set; } = string.Empty;

        public static OperationCategoryEntity FromDto(OperationCategory dto) => new()
        {
            Id = dto.Id,
            CategoryName = dto.CategoryName,
            CategoryType = dto.CategoryType
        };

        public OperationCategory ToDto() => new()
        {
            Id = this.Id,
            CategoryName = this.CategoryName,
            CategoryType = this.CategoryType
        };
    }
}
