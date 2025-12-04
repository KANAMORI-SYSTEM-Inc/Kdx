using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("ErrorMessage")]
    internal class ErrorMessageEntity : BaseModel
    {
        [PrimaryKey("MnemonicId")]
        [Column("MnemonicId")]
        public int MnemonicId { get; set; }

        [PrimaryKey("AlarmId")]
        [Column("AlarmId")]
        public int AlarmId { get; set; }

        [Column("BaseMessage")]
        public string? BaseMessage { get; set; }

        [Column("BaseAlarm")]
        public string? BaseAlarm { get; set; }

        [Column("Category1")]
        public string? Category1 { get; set; }

        [Column("Category2")]
        public string? Category2 { get; set; }

        [Column("Category3")]
        public string? Category3 { get; set; }

        [Column("DefaultCountTime")]
        public int DefaultCountTime { get; set; }

        public static ErrorMessageEntity FromDto(ErrorMessage dto) => new()
        {
            MnemonicId = dto.MnemonicId,
            AlarmId = dto.AlarmId,
            BaseMessage = dto.BaseMessage,
            BaseAlarm = dto.BaseAlarm,
            Category1 = dto.Category1,
            Category2 = dto.Category2,
            Category3 = dto.Category3,
            DefaultCountTime = dto.DefaultCountTime
        };

        public ErrorMessage ToDto() => new()
        {
            MnemonicId = this.MnemonicId,
            AlarmId = this.AlarmId,
            BaseMessage = this.BaseMessage,
            BaseAlarm = this.BaseAlarm,
            Category1 = this.Category1,
            Category2 = this.Category2,
            Category3 = this.Category3,
            DefaultCountTime = this.DefaultCountTime
        };
    }
}
