using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("ProcessCategory")]
    internal class ProcessCategoryEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ProcessCategoryName")]
        public string? ProcessCategoryName { get; set; }

        public static ProcessCategoryEntity FromDto(ProcessCategory dto) => new()
        {
            Id = dto.Id,
            ProcessCategoryName = dto.ProcessCategoryName
        };

        public ProcessCategory ToDto() => new()
        {
            Id = this.Id,
            ProcessCategoryName = this.ProcessCategoryName
        };
    }
}
