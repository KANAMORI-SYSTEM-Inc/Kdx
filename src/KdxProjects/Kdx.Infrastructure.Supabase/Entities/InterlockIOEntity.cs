using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("InterlockIO")]
    internal class InterlockIOEntity : BaseModel
    {
        [PrimaryKey("CylinderId")] // 複合主キー
        [Column("CylinderId")]
        public int CylinderId { get; set; }

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

        /// <summary>
        /// IO番号/インデックス (0から始まる)
        /// 同一InterlockCondition内での順序を示す
        /// </summary>
        [Column("IoIndex")]
        public int IoIndex { get; set; }

        public static InterlockIOEntity FromDto(InterlockIO dto) => new()
        {
            CylinderId = dto.CylinderId,
            PlcId = dto.PlcId,
            IOAddress = dto.IOAddress,
            InterlockSortId = dto.InterlockSortId,
            ConditionNumber = dto.ConditionNumber,
            IsOnCondition = dto.IsOnCondition,
            IoIndex = dto.IoIndex
        };

        public InterlockIO ToDto() => new()
        {
            CylinderId = this.CylinderId,
            PlcId = this.PlcId,
            IOAddress = this.IOAddress,
            InterlockSortId = this.InterlockSortId,
            ConditionNumber = this.ConditionNumber,
            IsOnCondition = this.IsOnCondition,
            IoIndex = this.IoIndex
        };
    }
}
