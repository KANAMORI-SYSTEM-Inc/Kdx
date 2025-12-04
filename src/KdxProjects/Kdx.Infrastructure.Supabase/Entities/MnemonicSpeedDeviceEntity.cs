using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("MnemonicSpeedDevice")]
    internal class MnemonicSpeedDeviceEntity : BaseModel
    {
        [PrimaryKey("ID")]
        [Column("ID")]
        public int ID { get; set; }

        [Column("CylinderId")]
        public int CylinderId { get; set; }

        [Column("Device")]
        public string Device { get; set; } = "D0";

        [Column("PlcId")]
        public int PlcId { get; set; }

        public static MnemonicSpeedDeviceEntity FromDto(MnemonicSpeedDevice dto) => new()
        {
            ID = dto.ID,
            CylinderId = dto.CylinderId,
            Device = dto.Device,
            PlcId = dto.PlcId
        };

        public MnemonicSpeedDevice ToDto() => new()
        {
            ID = this.ID,
            CylinderId = this.CylinderId,
            Device = this.Device,
            PlcId = this.PlcId
        };
    }
}
