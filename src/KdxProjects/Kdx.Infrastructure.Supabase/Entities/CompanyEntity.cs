using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Company")]
    internal class CompanyEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("CompanyName")]
        public string? CompanyName { get; set; }

        [Column("CreatedAt")]
        public string? CreatedAt { get; set; }

        // DTO → Entity
        public static CompanyEntity FromDto(Company dto) => new()
        {
            Id = dto.Id,
            CompanyName = dto.CompanyName,
            CreatedAt = dto.CreatedAt
        };

        // Entity → DTO
        public Company ToDto() => new()
        {
            Id = this.Id,
            CompanyName = this.CompanyName,
            CreatedAt = this.CreatedAt
        };
    }
}
