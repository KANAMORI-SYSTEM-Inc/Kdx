using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("InterlockIO")]
    internal class InterlockIOEntity : BaseModel
    {
        [PrimaryKey("InterlockId")] // 複合主キー
        [Column("InterlockId")]
        public int InterlockId { get; set; }

        [PrimaryKey("PlcId")] // 複合主キー
        [Column("PlcId")]
        public int PlcId { get; set; }

        [PrimaryKey("IOAddress")] // 複合主キー
        [Column("IOAddress")]
        public string IOAddress { get; set; } = string.Empty;

        [PrimaryKey("InterlockSortId")] // 複合主キー
        [Column("InterlockSortId")]
        public int InterlockSortId { get; set; }

        [PrimaryKey("ConditionNumber")] // 複合主キー
        [Column("ConditionNumber")]
        public int ConditionNumber { get; set; }

        [Column("IsOnCondition")]
        public bool IsOnCondition { get; set; }

        public static InterlockIOEntity FromDto(InterlockIO dto) => new()
        {
            InterlockId = dto.InterlockId,
            PlcId = dto.PlcId,
            IOAddress = dto.IOAddress,
            InterlockSortId = dto.InterlockSortId,
            ConditionNumber = dto.ConditionNumber,
            IsOnCondition = dto.IsOnCondition
        };

        public InterlockIO ToDto() => new()
        {
            InterlockId = this.InterlockId,
            PlcId = this.PlcId,
            IOAddress = this.IOAddress,
            InterlockSortId = this.InterlockSortId,
            ConditionNumber = this.ConditionNumber,
            IsOnCondition = this.IsOnCondition
        };
    }
}
