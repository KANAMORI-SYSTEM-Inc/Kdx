using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("CylinderCycle")]
    internal class CylinderCycleEntity : BaseModel
    {
        [PrimaryKey("CylinderId")]
        [Column("CylinderId")]
        public int CylinderId { get; set; }

        [Column("PlcId")]
        public int PlcId { get; set; }

        [PrimaryKey("CycleId")]
        [Column("CycleId")]
        public int CycleId { get; set; }

        public static CylinderCycleEntity FromDto(CylinderCycle dto) => new()
        {
            CylinderId = dto.CylinderId,
            PlcId = dto.PlcId,
            CycleId = dto.CycleId
        };

        public CylinderCycle ToDto() => new()
        {
            CylinderId = this.CylinderId,
            PlcId = this.PlcId,
            CycleId = this.CycleId
        };
    }
}
