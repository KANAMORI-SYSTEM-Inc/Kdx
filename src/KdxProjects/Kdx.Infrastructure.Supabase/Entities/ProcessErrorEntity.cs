using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("ProcessError")]
    internal class ProcessErrorEntity : BaseModel
    {
        [PrimaryKey("PlcId", false)]
        [Column("PlcId")]
        public int? PlcId { get; set; }

        [Column("CycleId")]
        public int? CycleId { get; set; }

        [Column("Device")]
        public string? Device { get; set; }

        [Column("MnemonicId")]
        public int? MnemonicId { get; set; }

        [Column("RecordId")]
        public int? RecordId { get; set; }

        [Column("AlarmId")]
        public int? AlarmId { get; set; }

        [PrimaryKey("ErrorNum", false)]
        [Column("ErrorNum")]
        public int? ErrorNum { get; set; }

        [Column("Comment1")]
        public string? Comment1 { get; set; }

        [Column("Comment2")]
        public string? Comment2 { get; set; }

        [Column("Comment3")]
        public string? Comment3 { get; set; }

        [Column("Comment4")]
        public string? Comment4 { get; set; }

        [Column("AlarmComment")]
        public string? AlarmComment { get; set; }

        [Column("MessageComment")]
        public string? MessageComment { get; set; }

        [Column("ErrorTime")]
        public int? ErrorTime { get; set; }

        [Column("ErrorTimeDevice")]
        public string? ErrorTimeDevice { get; set; }

        public static ProcessErrorEntity FromDto(ProcessError dto) => new()
        {
            PlcId = dto.PlcId,
            CycleId = dto.CycleId,
            Device = dto.Device,
            MnemonicId = dto.MnemonicId,
            RecordId = dto.RecordId,
            AlarmId = dto.AlarmId,
            ErrorNum = dto.ErrorNum,
            Comment1 = dto.Comment1,
            Comment2 = dto.Comment2,
            Comment3 = dto.Comment3,
            Comment4 = dto.Comment4,
            AlarmComment = dto.AlarmComment,
            MessageComment = dto.MessageComment,
            ErrorTime = dto.ErrorTime,
            ErrorTimeDevice = dto.ErrorTimeDevice
        };

        public ProcessError ToDto() => new()
        {
            PlcId = this.PlcId,
            CycleId = this.CycleId,
            Device = this.Device,
            MnemonicId = this.MnemonicId,
            RecordId = this.RecordId,
            AlarmId = this.AlarmId,
            ErrorNum = this.ErrorNum,
            Comment1 = this.Comment1,
            Comment2 = this.Comment2,
            Comment3 = this.Comment3,
            Comment4 = this.Comment4,
            AlarmComment = this.AlarmComment,
            MessageComment = this.MessageComment,
            ErrorTime = this.ErrorTime,
            ErrorTimeDevice = this.ErrorTimeDevice
        };
    }
}
