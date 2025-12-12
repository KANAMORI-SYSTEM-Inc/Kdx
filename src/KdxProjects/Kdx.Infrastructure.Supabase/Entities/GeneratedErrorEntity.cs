using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    /// <summary>
    /// GeneratedErrorテーブルのエンティティ
    /// 複合キー: (PlcId, ErrorNum)
    /// </summary>
    [Table("generated_error")]
    public class GeneratedErrorEntity : BaseModel
    {
        [PrimaryKey("plc_id", false)]
        [Column("plc_id")]
        public int PlcId { get; set; }

        [PrimaryKey("error_num", false)]
        [Column("error_num")]
        public int ErrorNum { get; set; }

        [Column("mnemonic_id")]
        public int MnemonicId { get; set; }

        [Column("alarm_id")]
        public int AlarmId { get; set; }

        [Column("record_id")]
        public int? RecordId { get; set; }

        [Column("device_m")]
        public string? DeviceM { get; set; }

        [Column("device_t")]
        public string? DeviceT { get; set; }

        [Column("comment1")]
        public string? Comment1 { get; set; }

        [Column("comment2")]
        public string? Comment2 { get; set; }

        [Column("comment3")]
        public string? Comment3 { get; set; }

        [Column("comment4")]
        public string? Comment4 { get; set; }

        [Column("alarm_comment")]
        public string? AlarmComment { get; set; }

        [Column("message_comment")]
        public string? MessageComment { get; set; }

        [Column("error_time")]
        public int ErrorTime { get; set; }

        [Column("cycle_id")]
        public int? CycleId { get; set; }

        /// <summary>
        /// DTOからエンティティを作成
        /// </summary>
        public static GeneratedErrorEntity FromDto(GeneratedError dto)
        {
            return new GeneratedErrorEntity
            {
                PlcId = dto.PlcId,
                ErrorNum = dto.ErrorNum,
                MnemonicId = dto.MnemonicId,
                AlarmId = dto.AlarmId,
                RecordId = dto.RecordId,
                DeviceM = dto.DeviceM,
                DeviceT = dto.DeviceT,
                Comment1 = dto.Comment1,
                Comment2 = dto.Comment2,
                Comment3 = dto.Comment3,
                Comment4 = dto.Comment4,
                AlarmComment = dto.AlarmComment,
                MessageComment = dto.MessageComment,
                ErrorTime = dto.ErrorTime,
                CycleId = dto.CycleId
            };
        }

        /// <summary>
        /// エンティティをDTOに変換
        /// </summary>
        public GeneratedError ToDto()
        {
            return new GeneratedError
            {
                PlcId = this.PlcId,
                ErrorNum = this.ErrorNum,
                MnemonicId = this.MnemonicId,
                AlarmId = this.AlarmId,
                RecordId = this.RecordId,
                DeviceM = this.DeviceM,
                DeviceT = this.DeviceT,
                Comment1 = this.Comment1,
                Comment2 = this.Comment2,
                Comment3 = this.Comment3,
                Comment4 = this.Comment4,
                AlarmComment = this.AlarmComment,
                MessageComment = this.MessageComment,
                ErrorTime = this.ErrorTime,
                CycleId = this.CycleId
            };
        }
    }
}
