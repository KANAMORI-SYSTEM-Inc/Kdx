using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Timer")]
    internal class TimerEntity : BaseModel
    {
        [PrimaryKey("ID")]
        [Column("ID")]
        public int ID { get; set; }

        [Column("CycleId")]
        public int? CycleId { get; set; }

        [Column("TimerCategoryId")]
        public int? TimerCategoryId { get; set; }

        [Column("TimerNum")]
        public int? TimerNum { get; set; }

        [Column("TimerName")]
        public string? TimerName { get; set; }

        [Column("MnemonicId")]
        public int? MnemonicId { get; set; }

        [Column("Example")]
        public int? Example { get; set; }

        public static TimerEntity FromDto(Kdx.Contracts.DTOs.Timer dto) => new()
        {
            ID = dto.ID,
            CycleId = dto.CycleId,
            TimerCategoryId = dto.TimerCategoryId,
            TimerNum = dto.TimerNum,
            TimerName = dto.TimerName,
            MnemonicId = dto.MnemonicId,
            Example = dto.Example
        };

        public Kdx.Contracts.DTOs.Timer ToDto() => new()
        {
            ID = this.ID,
            CycleId = this.CycleId,
            TimerCategoryId = this.TimerCategoryId,
            TimerNum = this.TimerNum,
            TimerName = this.TimerName,
            MnemonicId = this.MnemonicId,
            Example = this.Example
        };
    }
}
