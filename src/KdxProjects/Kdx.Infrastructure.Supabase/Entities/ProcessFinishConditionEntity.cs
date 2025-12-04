using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("ProcessFinishCondition")]
    internal class ProcessFinishConditionEntity : BaseModel
    {
        [PrimaryKey("ProcessId")]
        [Column("ProcessId")]
        public int ProcessId { get; set; }

        [PrimaryKey("FinishProcessDetailId")]
        [Column("FinishProcessDetailId")]
        public int FinishProcessDetailId { get; set; }

        [Column("CycleId")]
        public int? CycleId { get; set; }

        [Column("FinishSensor")]
        public string? FinishSensor { get; set; }

        public static ProcessFinishConditionEntity FromDto(ProcessFinishCondition dto) => new()
        {
            ProcessId = dto.ProcessId,
            FinishProcessDetailId = dto.FinishProcessDetailId,
            CycleId = dto.CycleId,
            FinishSensor = dto.FinishSensor
        };

        public ProcessFinishCondition ToDto() => new()
        {
            ProcessId = this.ProcessId,
            FinishProcessDetailId = this.FinishProcessDetailId,
            CycleId = this.CycleId,
            FinishSensor = this.FinishSensor
        };
    }
}
