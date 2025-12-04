using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("ProcessStartCondition")]
    internal class ProcessStartConditionEntity : BaseModel
    {
        [PrimaryKey("ProcessId")]
        [Column("ProcessId")]
        public int ProcessId { get; set; }

        [PrimaryKey("StartProcessDetailId")]
        [Column("StartProcessDetailId")]
        public int StartProcessDetailId { get; set; }

        [Column("CycleId")]
        public int? CycleId { get; set; }

        [Column("StartSensor")]
        public string? StartSensor { get; set; }

        public static ProcessStartConditionEntity FromDto(ProcessStartCondition dto) => new()
        {
            ProcessId = dto.ProcessId,
            StartProcessDetailId = dto.StartProcessDetailId,
            CycleId = dto.CycleId,
            StartSensor = dto.StartSensor
        };

        public ProcessStartCondition ToDto() => new()
        {
            ProcessId = this.ProcessId,
            StartProcessDetailId = this.StartProcessDetailId,
            CycleId = this.CycleId,
            StartSensor = this.StartSensor
        };
    }
}
