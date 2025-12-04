using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("IOHistory")]
    internal class IOHistoryEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("IoAddress")]
        public string? IoAddress { get; set; }

        [Column("IoPlcId")]
        public int? IoPlcId { get; set; }

        [Column("PropertyName")]
        public string? PropertyName { get; set; }

        [Column("OldValue")]
        public string? OldValue { get; set; }

        [Column("NewValue")]
        public string? NewValue { get; set; }

        [Column("ChangedAt")]
        public string? ChangedAt { get; set; }

        [Column("ChangedBy")]
        public string? ChangedBy { get; set; }

        public static IOHistoryEntity FromDto(IOHistory dto) => new()
        {
            Id = dto.Id,
            IoAddress = dto.IoAddress,
            IoPlcId = dto.IoPlcId,
            PropertyName = dto.PropertyName,
            OldValue = dto.OldValue,
            NewValue = dto.NewValue,
            ChangedAt = dto.ChangedAt,
            ChangedBy = dto.ChangedBy
        };

        public IOHistory ToDto() => new()
        {
            Id = this.Id,
            IoAddress = this.IoAddress,
            IoPlcId = this.IoPlcId,
            PropertyName = this.PropertyName,
            OldValue = this.OldValue,
            NewValue = this.NewValue,
            ChangedAt = this.ChangedAt,
            ChangedBy = this.ChangedBy
        };
    }
}
