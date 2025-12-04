using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Cycle")]
    internal class CycleEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("CycleName")]
        public string? CycleName { get; set; }

        [Column("StartDevice")]
        public string StartDevice { get; set; } = "L1000";

        [Column("ResetDevice")]
        public string ResetDevice { get; set; } = "L1001";

        [Column("PauseDevice")]
        public string PauseDevice { get; set; } = "L1002";

        public static CycleEntity FromDto(Cycle dto) => new()
        {
            Id = dto.Id,
            PlcId = dto.PlcId,
            CycleName = dto.CycleName,
            StartDevice = dto.StartDevice,
            ResetDevice = dto.ResetDevice,
            PauseDevice = dto.PauseDevice
        };

        public Cycle ToDto() => new()
        {
            Id = this.Id,
            PlcId = this.PlcId,
            CycleName = this.CycleName,
            StartDevice = this.StartDevice,
            ResetDevice = this.ResetDevice,
            PauseDevice = this.PauseDevice
        };
    }
}
