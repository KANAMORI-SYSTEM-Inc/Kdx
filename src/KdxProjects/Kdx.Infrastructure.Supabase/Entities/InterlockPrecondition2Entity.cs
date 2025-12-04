using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("InterlockPrecondition2")]
    internal class InterlockPrecondition2Entity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("IsEnableProcess")]
        public bool IsEnableProcess { get; set; }

        [Column("InterlockMode")]
        public string? InterlockMode { get; set; }

        [Column("StartDetailId")]
        public int? StartDetailId { get; set; }

        [Column("EndDetailId")]
        public int? EndDetailId { get; set; }

        public static InterlockPrecondition2Entity FromDto(InterlockPrecondition2 dto) => new()
        {
            Id = dto.Id,
            IsEnableProcess = dto.IsEnableProcess,
            InterlockMode = dto.InterlockMode,
            StartDetailId = dto.StartDetailId,
            EndDetailId = dto.EndDetailId
        };

        public InterlockPrecondition2 ToDto() => new()
        {
            Id = this.Id,
            IsEnableProcess = this.IsEnableProcess,
            InterlockMode = this.InterlockMode,
            StartDetailId = this.StartDetailId,
            EndDetailId = this.EndDetailId
        };
    }
}
