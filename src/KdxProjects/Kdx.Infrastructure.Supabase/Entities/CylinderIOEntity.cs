using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("CylinderIO")]
    internal class CylinderIOEntity : BaseModel
    {
        [PrimaryKey("CylinderId")]
        [Column("CylinderId")]
        public int CylinderId { get; set; }

        [PrimaryKey("IOAddress")]
        [Column("IOAddress")]
        public string IOAddress { get; set; } = string.Empty;

        [PrimaryKey("PlcId")]
        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("IOType")]
        public string IOType { get; set; } = string.Empty;

        [Column("SortOrder")]
        public int? SortOrder { get; set; }

        [Column("Comment")]
        public string? Comment { get; set; }

        public static CylinderIOEntity FromDto(CylinderIO dto) => new()
        {
            CylinderId = dto.CylinderId,
            IOAddress = dto.IOAddress,
            PlcId = dto.PlcId,
            IOType = dto.IOType,
            SortOrder = dto.SortOrder,
            Comment = dto.Comment
        };

        public CylinderIO ToDto() => new()
        {
            CylinderId = this.CylinderId,
            IOAddress = this.IOAddress,
            PlcId = this.PlcId,
            IOType = this.IOType,
            SortOrder = this.SortOrder,
            Comment = this.Comment
        };
    }
}
