using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Length")]
    internal class LengthEntity : BaseModel
    {
        [PrimaryKey("ID")]
        [Column("ID")]
        public int ID { get; set; }

        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("LengthName")]
        public string? LengthName { get; set; }

        [Column("Device")]
        public string? Device { get; set; }

        public static LengthEntity FromDto(Length dto) => new()
        {
            ID = dto.ID,
            PlcId = dto.PlcId,
            LengthName = dto.LengthName,
            Device = dto.Device
        };

        public Length ToDto() => new()
        {
            ID = this.ID,
            PlcId = this.PlcId,
            LengthName = this.LengthName,
            Device = this.Device
        };
    }
}
