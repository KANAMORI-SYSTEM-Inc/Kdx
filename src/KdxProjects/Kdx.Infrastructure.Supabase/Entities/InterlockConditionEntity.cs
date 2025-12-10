using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("InterlockCondition")]
    internal class InterlockConditionEntity : BaseModel
    {
        [PrimaryKey("CylinderId")] // 複合主キー
        [Column("CylinderId")]
        public int CylinderId { get; set; }

        [PrimaryKey("ConditionNumber")] // 複合主キー
        [Column("ConditionNumber")]
        public int ConditionNumber { get; set; }

        [PrimaryKey("InterlockSortId")] // 複合主キー
        [Column("InterlockSortId")]
        public int InterlockSortId { get; set; }

        [Column("ConditionTypeId")]
        public int? ConditionTypeId { get; set; }

        [Column("Name")]
        public string? Name { get; set; }

        [Column("Device")]
        public string? Device { get; set; }

        [Column("IsOnCondition")]
        public bool? IsOnCondition { get; set; }

        public static InterlockConditionEntity FromDto(InterlockCondition dto) => new()
        {
            CylinderId = dto.CylinderId,
            ConditionNumber = dto.ConditionNumber,
            InterlockSortId = dto.InterlockSortId,
            ConditionTypeId = dto.ConditionTypeId,
            Name = dto.Name,
            Device = dto.Device,
            IsOnCondition = dto.IsOnCondition
        };

        public InterlockCondition ToDto() => new()
        {
            CylinderId = this.CylinderId,
            ConditionNumber = this.ConditionNumber,
            InterlockSortId = this.InterlockSortId,
            ConditionTypeId = this.ConditionTypeId,
            Name = this.Name,
            Device = this.Device,
            IsOnCondition = this.IsOnCondition
        };
    }
}
