using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("PLC")]
    internal class PLCEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("PlcName")]
        public string? PlcName { get; set; }

        [Column("ModelId")]
        public int? ModelId { get; set; }

        [Column("Maker")]
        public string? Maker { get; set; }

        public static PLCEntity FromDto(PLC dto) => new()
        {
            Id = dto.Id,
            PlcName = dto.PlcName,
            ModelId = dto.ModelId,
            Maker = dto.Maker
        };

        public PLC ToDto() => new()
        {
            Id = this.Id,
            PlcName = this.PlcName,
            ModelId = this.ModelId,
            Maker = this.Maker
        };
    }
}
