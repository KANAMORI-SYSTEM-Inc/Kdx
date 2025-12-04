using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Interlock")]
    internal class InterlockEntity : BaseModel
    {
        [PrimaryKey("CylinderId")] // 複合主キー
        [Column("CylinderId")]
        public int CylinderId { get; set; }

        [PrimaryKey("SortId")] // 複合主キー
        [Column("SortId")]
        public int SortId { get; set; }

        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("ConditionCylinderId")]
        public int ConditionCylinderId { get; set; }

        [Column("PreConditionID1")]
        public int? PreConditionID1 { get; set; }

        [Column("PreConditionID2")]
        public int? PreConditionID2 { get; set; }

        [Column("GoOrBack")]
        public int GoOrBack { get; set; }

        public static InterlockEntity FromDto(Interlock dto) => new()
        {
            CylinderId = dto.CylinderId,
            SortId = dto.SortId,
            PlcId = dto.PlcId,
            ConditionCylinderId = dto.ConditionCylinderId,
            PreConditionID1 = dto.PreConditionID1,
            PreConditionID2 = dto.PreConditionID2,
            GoOrBack = dto.GoOrBack
        };

        public Interlock ToDto() => new()
        {
            CylinderId = this.CylinderId,
            SortId = this.SortId,
            PlcId = this.PlcId,
            ConditionCylinderId = this.ConditionCylinderId,
            PreConditionID1 = this.PreConditionID1,
            PreConditionID2 = this.PreConditionID2,
            GoOrBack = this.GoOrBack
        };
    }
}
