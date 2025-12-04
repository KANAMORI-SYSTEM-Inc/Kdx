using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("CylinderControlBox")]
    internal class CylinderControlBoxEntity : BaseModel
    {
        [PrimaryKey("CylinderId")]
        [Column("CylinderId")]
        public int CylinderId { get; set; }

        [PrimaryKey("PlcId")]
        [Column("PlcId")]
        public int PlcId { get; set; }

        [PrimaryKey("ManualNumber")]
        [Column("ManualNumber")]
        public int ManualNumber { get; set; }

        [Column("Device")]
        public string? Device { get; set; }

        public static CylinderControlBoxEntity FromDto(CylinderControlBox dto) => new()
        {
            CylinderId = dto.CylinderId,
            PlcId = dto.PlcId,
            ManualNumber = dto.ManualNumber,
            Device = dto.Device
        };

        public CylinderControlBox ToDto() => new()
        {
            CylinderId = this.CylinderId,
            PlcId = this.PlcId,
            ManualNumber = this.ManualNumber,
            Device = this.Device
        };
    }
}
