using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("OperationIO")]
    internal class OperationIOEntity : BaseModel
    {
        [PrimaryKey("OperationId")]
        [Column("OperationId")]
        public int OperationId { get; set; }

        [PrimaryKey("IOAddress")]
        [Column("IOAddress")]
        public string IOAddress { get; set; } = string.Empty;

        [PrimaryKey("PlcId")]
        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("IOUsage")]
        public string IOUsage { get; set; } = string.Empty;

        [Column("SortOrder")]
        public int? SortOrder { get; set; }

        [Column("Comment")]
        public string? Comment { get; set; }

        public static OperationIOEntity FromDto(OperationIO dto) => new()
        {
            OperationId = dto.OperationId,
            IOAddress = dto.IOAddress,
            PlcId = dto.PlcId,
            IOUsage = dto.IOUsage,
            SortOrder = dto.SortOrder,
            Comment = dto.Comment
        };

        public OperationIO ToDto() => new()
        {
            OperationId = this.OperationId,
            IOAddress = this.IOAddress,
            PlcId = this.PlcId,
            IOUsage = this.IOUsage,
            SortOrder = this.SortOrder,
            Comment = this.Comment
        };
    }
}
