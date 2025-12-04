using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Model")]
    internal class ModelEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ModelName")]
        public string? ModelName { get; set; }

        [Column("CompanyId")]
        public int? CompanyId { get; set; }

        public static ModelEntity FromDto(Model dto) => new()
        {
            Id = dto.Id,
            ModelName = dto.ModelName,
            CompanyId = dto.CompanyId
        };

        public Model ToDto() => new()
        {
            Id = this.Id,
            ModelName = this.ModelName,
            CompanyId = this.CompanyId
        };
    }
}
