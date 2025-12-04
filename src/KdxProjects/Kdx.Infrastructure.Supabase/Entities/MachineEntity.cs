using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Machine")]
    internal class MachineEntity : BaseModel
    {
        [PrimaryKey("MachineNameId")]
        [Column("MachineNameId")]
        public int MachineNameId { get; set; }

        [PrimaryKey("DriveSubId")]
        [Column("DriveSubId")]
        public int DriveSubId { get; set; }

        [Column("UseValveRetention")]
        public bool UseValveRetention { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        public static MachineEntity FromDto(Machine dto) => new()
        {
            MachineNameId = dto.MachineNameId,
            DriveSubId = dto.DriveSubId,
            UseValveRetention = dto.UseValveRetention,
            Description = dto.Description
        };

        public Machine ToDto() => new()
        {
            MachineNameId = this.MachineNameId,
            DriveSubId = this.DriveSubId,
            UseValveRetention = this.UseValveRetention,
            Description = this.Description
        };
    }
}
