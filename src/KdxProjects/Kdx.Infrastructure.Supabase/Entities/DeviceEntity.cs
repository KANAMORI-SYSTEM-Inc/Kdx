using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Device")]
    internal class DeviceEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("DeviceName")]
        public string? DeviceName { get; set; }

        [Column("ModelId")]
        public int? ModelId { get; set; }

        public static DeviceEntity FromDto(Device dto) => new()
        {
            Id = dto.Id,
            DeviceName = dto.DeviceName,
            ModelId = dto.ModelId
        };

        public Device ToDto() => new()
        {
            Id = this.Id,
            DeviceName = this.DeviceName,
            ModelId = this.ModelId
        };
    }
}
