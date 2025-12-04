using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("MemoryCategory")]
    internal class MemoryCategoryEntity : BaseModel
    {
        [PrimaryKey("ID")]
        [Column("ID")]
        public int ID { get; set; }

        [Column("CategoryName")]
        public string? CategoryName { get; set; }

        [Column("Enum")]
        public string? Enum { get; set; }

        public static MemoryCategoryEntity FromDto(MemoryCategory dto) => new()
        {
            ID = dto.ID,
            CategoryName = dto.CategoryName,
            Enum = dto.Enum
        };

        public MemoryCategory ToDto() => new()
        {
            ID = this.ID,
            CategoryName = this.CategoryName,
            Enum = this.Enum
        };
    }
}
