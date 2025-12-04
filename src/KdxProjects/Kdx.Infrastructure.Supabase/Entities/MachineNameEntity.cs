using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("MachineName")]
    internal class MachineNameEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("FullName")]
        public string FullName { get; set; } = string.Empty;

        [Column("ShortName")]
        public string ShortName { get; set; } = string.Empty;

        public static MachineNameEntity FromDto(MachineName dto) => new()
        {
            Id = dto.Id,
            FullName = dto.FullName,
            ShortName = dto.ShortName
        };

        public MachineName ToDto() => new()
        {
            Id = this.Id,
            FullName = this.FullName,
            ShortName = this.ShortName
        };
    }
}
