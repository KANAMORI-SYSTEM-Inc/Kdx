using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("ControlBox")]
    internal class ControlBoxEntity : BaseModel
    {
        [PrimaryKey("PlcId")]
        [Column("PlcId")]
        public int PlcId { get; set; }

        [PrimaryKey("BoxNumber")]
        [Column("BoxNumber")]
        public int BoxNumber { get; set; }

        [Column("ManualMode")]
        public string ManualMode { get; set; } = string.Empty;

        [Column("ManualButton")]
        public string ManualButton { get; set; } = string.Empty;

        [Column("ErrorReset")]
        public string? ErrorReset { get; set; }

        public static ControlBoxEntity FromDto(ControlBox dto) => new()
        {
            PlcId = dto.PlcId,
            BoxNumber = dto.BoxNumber,
            ManualMode = dto.ManualMode,
            ManualButton = dto.ManualButton,
            ErrorReset = dto.ErrorReset
        };

        public ControlBox ToDto() => new()
        {
            PlcId = this.PlcId,
            BoxNumber = this.BoxNumber,
            ManualMode = this.ManualMode,
            ManualButton = this.ManualButton,
            ErrorReset = this.ErrorReset
        };
    }
}
