using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Servo")]
    internal class ServoEntity : BaseModel
    {
        [PrimaryKey("ID")]
        [Column("ID")]
        public int ID { get; set; }

        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("CylinderId")]
        public int CylinderId { get; set; }

        [Column("Busy")]
        public string Busy { get; set; } = string.Empty;

        [Column("PositioningStart")]
        public string PositioningStart { get; set; } = string.Empty;

        [Column("Prefix")]
        public string Prefix { get; set; } = string.Empty;

        [Column("AxisNumber")]
        public int AxisNumber { get; set; }

        [Column("AxisStop")]
        public string AxisStop { get; set; } = string.Empty;

        [Column("PositioningStartNum")]
        public string PositioningStartNum { get; set; } = string.Empty;

        [Column("GS")]
        public string GS { get; set; } = string.Empty;

        [Column("JogSpeed")]
        public string JogSpeed { get; set; } = string.Empty;

        [Column("StartFowardJog")]
        public string StartFowardJog { get; set; } = string.Empty;

        [Column("StartReverseJog")]
        public string StartReverseJog { get; set; } = string.Empty;

        [Column("CommandPosition")]
        public string CommandPosition { get; set; } = string.Empty;

        [Column("CurrentValue")]
        public string CurrentValue { get; set; } = string.Empty;

        [Column("OriginalPosition")]
        public string OriginalPosition { get; set; } = string.Empty;

        [Column("Status")]
        public string Status { get; set; } = string.Empty;

        public static ServoEntity FromDto(Servo dto) => new()
        {
            ID = dto.ID,
            PlcId = dto.PlcId,
            CylinderId = dto.CylinderId,
            Busy = dto.Busy,
            PositioningStart = dto.PositioningStart,
            Prefix = dto.Prefix,
            AxisNumber = dto.AxisNumber,
            AxisStop = dto.AxisStop,
            PositioningStartNum = dto.PositioningStartNum,
            GS = dto.GS,
            JogSpeed = dto.JogSpeed,
            StartFowardJog = dto.StartFowardJog,
            StartReverseJog = dto.StartReverseJog,
            CommandPosition = dto.CommandPosition,
            CurrentValue = dto.CurrentValue,
            OriginalPosition = dto.OriginalPosition,
            Status = dto.Status
        };

        public Servo ToDto() => new()
        {
            ID = this.ID,
            PlcId = this.PlcId,
            CylinderId = this.CylinderId,
            Busy = this.Busy,
            PositioningStart = this.PositioningStart,
            Prefix = this.Prefix,
            AxisNumber = this.AxisNumber,
            AxisStop = this.AxisStop,
            PositioningStartNum = this.PositioningStartNum,
            GS = this.GS,
            JogSpeed = this.JogSpeed,
            StartFowardJog = this.StartFowardJog,
            StartReverseJog = this.StartReverseJog,
            CommandPosition = this.CommandPosition,
            CurrentValue = this.CurrentValue,
            OriginalPosition = this.OriginalPosition,
            Status = this.Status
        };
    }
}
