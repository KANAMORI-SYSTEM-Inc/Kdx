using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("DriveSub")]
    internal class DriveSubEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("DriveSubName")]
        public string? DriveSubName { get; set; }

        [Column("DriveMainId")]
        public int? DriveMainId { get; set; }

        public static DriveSubEntity FromDto(DriveSub dto) => new()
        {
            Id = dto.Id,
            DriveSubName = dto.DriveSubName,
            DriveMainId = dto.DriveMainId
        };

        public DriveSub ToDto() => new()
        {
            Id = this.Id,
            DriveSubName = this.DriveSubName,
            DriveMainId = this.DriveMainId
        };
    }
}
