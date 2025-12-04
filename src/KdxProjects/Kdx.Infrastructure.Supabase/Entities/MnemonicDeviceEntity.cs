using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("MnemonicDevice")]
    internal class MnemonicDeviceEntity : BaseModel
    {
        [PrimaryKey("ID")]
        [Column("ID")]
        public long? ID { get; set; }

        [Column("MnemonicId")]
        public int MnemonicId { get; set; }

        [Column("RecordId")]
        public int RecordId { get; set; }

        [Column("DeviceLabel")]
        public string DeviceLabel { get; set; } = "L";

        [Column("StartNum")]
        public int StartNum { get; set; }

        [Column("OutCoilCount")]
        public int OutCoilCount { get; set; }

        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("Comment1")]
        public string? Comment1 { get; set; }

        [Column("Comment2")]
        public string? Comment2 { get; set; }

        public static MnemonicDeviceEntity FromDto(MnemonicDevice dto) => new()
        {
            ID = dto.ID,
            MnemonicId = dto.MnemonicId,
            RecordId = dto.RecordId,
            DeviceLabel = dto.DeviceLabel,
            StartNum = dto.StartNum,
            OutCoilCount = dto.OutCoilCount,
            PlcId = dto.PlcId,
            Comment1 = dto.Comment1,
            Comment2 = dto.Comment2
        };

        public MnemonicDevice ToDto() => new()
        {
            ID = this.ID,
            MnemonicId = this.MnemonicId,
            RecordId = this.RecordId,
            DeviceLabel = this.DeviceLabel,
            StartNum = this.StartNum,
            OutCoilCount = this.OutCoilCount,
            PlcId = this.PlcId,
            Comment1 = this.Comment1,
            Comment2 = this.Comment2
        };
    }
}
