using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("IO")]
    internal class IOEntity : BaseModel
    {
        [PrimaryKey("Address")]
        [Column("Address")]
        public string Address { get; set; } = string.Empty;

        [PrimaryKey("PlcId")]
        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("IOText")]
        public string? IOText { get; set; }

        [Column("XComment")]
        public string? XComment { get; set; }

        [Column("YComment")]
        public string? YComment { get; set; }

        [Column("FComment")]
        public string? FComment { get; set; }

        [Column("IOName")]
        public string? IOName { get; set; }

        [Column("IOExplanation")]
        public string? IOExplanation { get; set; }

        [Column("IOSpot")]
        public string? IOSpot { get; set; }

        [Column("UnitName")]
        public string? UnitName { get; set; }

        [Column("System")]
        public string? System { get; set; }

        [Column("StationNumber")]
        public string? StationNumber { get; set; }

        [Column("IONameNaked")]
        public string? IONameNaked { get; set; }

        [Column("LinkDevice")]
        public string? LinkDevice { get; set; }

        [Column("IOType")]
        public int? IOType { get; set; }

        [Column("IsInverted")]
        public bool? IsInverted { get; set; }

        [Column("IsEnabled")]
        public bool? IsEnabled { get; set; }

        public static IOEntity FromDto(IO dto) => new()
        {
            Address = dto.Address,
            PlcId = dto.PlcId,
            IOText = dto.IOText,
            XComment = dto.XComment,
            YComment = dto.YComment,
            FComment = dto.FComment,
            IOName = dto.IOName,
            IOExplanation = dto.IOExplanation,
            IOSpot = dto.IOSpot,
            UnitName = dto.UnitName,
            System = dto.System,
            StationNumber = dto.StationNumber,
            IONameNaked = dto.IONameNaked,
            LinkDevice = dto.LinkDevice,
            IOType = dto.IOType,
            IsInverted = dto.IsInverted ?? false,
            IsEnabled = dto.IsEnabled ?? true
        };

        public IO ToDto() => new()
        {
            Address = this.Address,
            PlcId = this.PlcId,
            IOText = this.IOText,
            XComment = this.XComment,
            YComment = this.YComment,
            FComment = this.FComment,
            IOName = this.IOName,
            IOExplanation = this.IOExplanation,
            IOSpot = this.IOSpot,
            UnitName = this.UnitName,
            System = this.System,
            StationNumber = this.StationNumber,
            IONameNaked = this.IONameNaked,
            LinkDevice = this.LinkDevice,
            IOType = this.IOType,
            IsInverted = this.IsInverted,
            IsEnabled = this.IsEnabled
        };
    }
}
