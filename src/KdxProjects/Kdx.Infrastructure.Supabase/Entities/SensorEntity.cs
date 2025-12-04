using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Sensor")]
    internal class SensorEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("SensorName")]
        public string? SensorName { get; set; }

        public static SensorEntity FromDto(Sensor dto) => new()
        {
            Id = dto.Id,
            SensorName = dto.SensorName
        };

        public Sensor ToDto() => new()
        {
            Id = this.Id,
            SensorName = this.SensorName
        };
    }
}
