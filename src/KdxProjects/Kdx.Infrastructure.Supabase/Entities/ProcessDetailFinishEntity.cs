using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("ProcessDetailFinish")]
    internal class ProcessDetailFinishEntity : BaseModel
    {
        [PrimaryKey("ProcessDetailId")]
        [Column("ProcessDetailId")]
        public int ProcessDetailId { get; set; }

        [PrimaryKey("FinishProcessDetailId")]
        [Column("FinishProcessDetailId")]
        public int FinishProcessDetailId { get; set; }

        [Column("CycleId")]
        public int CycleId { get; set; }

        public static ProcessDetailFinishEntity FromDto(ProcessDetailFinish dto) => new()
        {
            ProcessDetailId = dto.ProcessDetailId,
            FinishProcessDetailId = dto.FinishProcessDetailId,
            CycleId = dto.CycleId
        };

        public ProcessDetailFinish ToDto() => new()
        {
            ProcessDetailId = this.ProcessDetailId,
            FinishProcessDetailId = this.FinishProcessDetailId,
            CycleId = this.CycleId
        };
    }
}
