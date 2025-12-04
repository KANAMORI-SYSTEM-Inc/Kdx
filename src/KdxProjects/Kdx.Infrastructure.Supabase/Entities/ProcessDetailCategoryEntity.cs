using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("ProcessDetailCategory")]
    internal class ProcessDetailCategoryEntity : BaseModel
    {
        [PrimaryKey("ID")]
        [Column("ID")]
        public int Id { get; set; }

        [Column("CategoryName")]
        public string? CategoryName { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("ShortName")]
        public string? ShortName { get; set; }

        public static ProcessDetailCategoryEntity FromDto(ProcessDetailCategory dto) => new()
        {
            Id = dto.Id,
            CategoryName = dto.CategoryName,
            Description = dto.Description,
            ShortName = dto.ShortName
        };

        public ProcessDetailCategory ToDto() => new()
        {
            Id = this.Id,
            CategoryName = this.CategoryName,
            Description = this.Description,
            ShortName = this.ShortName
        };
    }
}
