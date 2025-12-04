using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("MnemonicTimerDevice")]
    internal class MnemonicTimerDeviceEntity : BaseModel
    {
        [PrimaryKey("MnemonicId")]
        [Column("MnemonicId")]
        public int MnemonicId { get; set; }

        [PrimaryKey("RecordId")]
        [Column("RecordId")]
        public int RecordId { get; set; }

        [PrimaryKey("TimerId")]
        [Column("TimerId")]
        public int TimerId { get; set; }

        [Column("TimerCategoryId")]
        public int? TimerCategoryId { get; set; }

        [Column("TimerDeviceT")]
        public string TimerDeviceT { get; set; } = "T0";

        [Column("TimerDeviceZR")]
        public string TimerDeviceZR { get; set; } = "ZR0";

        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("CycleId")]
        public int? CycleId { get; set; }

        [Column("Comment1")]
        public string? Comment1 { get; set; }

        [Column("Comment2")]
        public string? Comment2 { get; set; }

        [Column("Comment3")]
        public string? Comment3 { get; set; }

        public static MnemonicTimerDeviceEntity FromDto(MnemonicTimerDevice dto) => new()
        {
            MnemonicId = dto.MnemonicId,
            RecordId = dto.RecordId,
            TimerId = dto.TimerId,
            TimerCategoryId = dto.TimerCategoryId,
            TimerDeviceT = dto.TimerDeviceT,
            TimerDeviceZR = dto.TimerDeviceZR,
            PlcId = dto.PlcId,
            CycleId = dto.CycleId,
            Comment1 = dto.Comment1,
            Comment2 = dto.Comment2,
            Comment3 = dto.Comment3
        };

        public MnemonicTimerDevice ToDto() => new()
        {
            MnemonicId = this.MnemonicId,
            RecordId = this.RecordId,
            TimerId = this.TimerId,
            TimerCategoryId = this.TimerCategoryId,
            TimerDeviceT = this.TimerDeviceT,
            TimerDeviceZR = this.TimerDeviceZR,
            PlcId = this.PlcId,
            CycleId = this.CycleId,
            Comment1 = this.Comment1,
            Comment2 = this.Comment2,
            Comment3 = this.Comment3
        };
    }
}
