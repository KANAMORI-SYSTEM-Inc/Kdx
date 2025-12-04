using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("v_interlock_conditions")]
    internal class ViewInterlockConditionsEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("CylinderId")]
        public int CylinderId { get; set; }

        [Column("InterlockId")]
        public int InterlockId { get; set; }

        [Column("InterlockName")]
        public string InterlockName { get; set; } = string.Empty;

        [Column("ConditionNumber")]
        public int ConditionNumber { get; set; }

        [Column("ConditionTypeId")]
        public int ConditionTypeId { get; set; }

        [Column("ConditionTypeName")]
        public string ConditionTypeName { get; set; } = string.Empty;

        public static ViewInterlockConditionsEntity FromDto(ViewInterlockConditions dto) => new()
        {
            Id = dto.Id,
            PlcId = dto.PlcId,
            CylinderId = dto.CylinderId,
            InterlockId = dto.InterlockId,
            InterlockName = dto.InterlockName,
            ConditionNumber = dto.ConditionNumber,
            ConditionTypeId = dto.ConditionTypeId,
            ConditionTypeName = dto.ConditionTypeName
        };

        public ViewInterlockConditions ToDto() => new()
        {
            Id = this.Id,
            PlcId = this.PlcId,
            CylinderId = this.CylinderId,
            InterlockId = this.InterlockId,
            InterlockName = this.InterlockName,
            ConditionNumber = this.ConditionNumber,
            ConditionTypeId = this.ConditionTypeId,
            ConditionTypeName = this.ConditionTypeName
        };
    }
}
