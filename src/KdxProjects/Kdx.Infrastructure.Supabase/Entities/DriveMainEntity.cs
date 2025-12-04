using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("DriveMain")]
    internal class DriveMainEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("DriveMainName")]
        public string? DriveMainName { get; set; }

        public static DriveMainEntity FromDto(DriveMain dto) => new()
        {
            Id = dto.Id,
            DriveMainName = dto.DriveMainName
        };

        public DriveMain ToDto() => new()
        {
            Id = this.Id,
            DriveMainName = this.DriveMainName
        };
    }
}
