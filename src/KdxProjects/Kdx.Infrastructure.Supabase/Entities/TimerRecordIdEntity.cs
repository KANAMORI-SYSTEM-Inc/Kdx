using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("TimerRecordIds")]
    internal class TimerRecordIdEntity : BaseModel
    {
        [PrimaryKey("TimerId")]
        [Column("TimerId")]
        public int TimerId { get; set; }

        [PrimaryKey("RecordId")]
        [Column("RecordId")]
        public int RecordId { get; set; }

        public static TimerRecordIdEntity FromDto(TimerRecordId dto) => new()
        {
            TimerId = dto.TimerId,
            RecordId = dto.RecordId
        };

        public TimerRecordId ToDto() => new()
        {
            TimerId = this.TimerId,
            RecordId = this.RecordId
        };
    }
}
