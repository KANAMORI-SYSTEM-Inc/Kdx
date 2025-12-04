using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("ProcessDetailConnection")]
    internal class ProcessDetailConnectionEntity : BaseModel
    {
        [PrimaryKey("FromProcessDetailId")]
        [Column("FromProcessDetailId")]
        public int FromProcessDetailId { get; set; }

        [PrimaryKey("ToProcessDetailId")]
        [Column("ToProcessDetailId")]
        public int? ToProcessDetailId { get; set; }

        [Column("CycleId")]
        public int CycleId { get; set; }

        public static ProcessDetailConnectionEntity FromDto(ProcessDetailConnection dto) => new()
        {
            FromProcessDetailId = dto.FromProcessDetailId,
            ToProcessDetailId = dto.ToProcessDetailId,
            CycleId = dto.CycleId ?? 0  // CycleIdが必須になったため、nullの場合は0をデフォルト値とする
        };

        public ProcessDetailConnection ToDto() => new()
        {
            FromProcessDetailId = this.FromProcessDetailId,
            ToProcessDetailId = this.ToProcessDetailId,
            CycleId = this.CycleId
        };
    }
}
