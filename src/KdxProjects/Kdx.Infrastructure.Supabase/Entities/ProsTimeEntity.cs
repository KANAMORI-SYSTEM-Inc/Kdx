using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("ProsTime")]
    internal class ProsTimeEntity : BaseModel
    {
        [PrimaryKey("PlcId")]
        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("MnemonicId")]
        public int MnemonicId { get; set; }

        [PrimaryKey("RecordId")]
        [Column("RecordId")]
        public int RecordId { get; set; }

        [PrimaryKey("SortId")]
        [Column("SortId")]
        public int SortId { get; set; }

        [Column("CurrentDevice")]
        public string CurrentDevice { get; set; } = "ZR0";

        [Column("PreviousDevice")]
        public string PreviousDevice { get; set; } = "ZR0";

        [Column("CylinderDevice")]
        public string CylinderDevice { get; set; } = "ZR0";

        [Column("CategoryId")]
        public int CategoryId { get; set; }

        [Column("OutcoilNumber")]
        public int OutcoilNumber { get; set; }

        public static ProsTimeEntity FromDto(ProsTime dto) => new()
        {
            PlcId = dto.PlcId,
            MnemonicId = dto.MnemonicId,
            RecordId = dto.RecordId,
            SortId = dto.SortId,
            CurrentDevice = dto.CurrentDevice,
            PreviousDevice = dto.PreviousDevice,
            CylinderDevice = dto.CylinderDevice,
            CategoryId = dto.CategoryId,
            OutcoilNumber = dto.OutcoilNumber
        };

        public ProsTime ToDto() => new()
        {
            PlcId = this.PlcId,
            MnemonicId = this.MnemonicId,
            RecordId = this.RecordId,
            SortId = this.SortId,
            CurrentDevice = this.CurrentDevice,
            PreviousDevice = this.PreviousDevice,
            CylinderDevice = this.CylinderDevice,
            CategoryId = this.CategoryId,
            OutcoilNumber = this.OutcoilNumber
        };
    }
}
