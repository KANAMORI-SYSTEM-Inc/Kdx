using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Memory")]
    internal class MemoryEntity : BaseModel
    {
        [PrimaryKey("PlcId")]
        [Column("PlcId")]
        public int PlcId { get; set; }

        [PrimaryKey("Device")]
        [Column("Device")]
        public string Device { get; set; } = string.Empty;

        [Column("MemoryCategory")]
        public int? MemoryCategory { get; set; }

        [Column("DeviceNumber")]
        public int? DeviceNumber { get; set; }

        [Column("DeviceNumber1")]
        public string? DeviceNumber1 { get; set; }

        [Column("DeviceNumber2")]
        public string? DeviceNumber2 { get; set; }

        [Column("Category")]
        public string? Category { get; set; }

        [Column("Row_1")]
        public string? Row_1 { get; set; }

        [Column("Row_2")]
        public string? Row_2 { get; set; }

        [Column("Row_3")]
        public string? Row_3 { get; set; }

        [Column("Row_4")]
        public string? Row_4 { get; set; }

        [Column("Direct_Input")]
        public string? Direct_Input { get; set; }

        [Column("Confirm")]
        public string? Confirm { get; set; }

        [Column("Note")]
        public string? Note { get; set; }

        [Column("GOT")]
        public string? GOT { get; set; }

        [Column("MnemonicId")]
        public int? MnemonicId { get; set; }

        [Column("RecordId")]
        public int? RecordId { get; set; }

        [Column("OutcoilNumber")]
        public int? OutcoilNumber { get; set; }

        public static MemoryEntity FromDto(Memory dto) => new()
        {
            PlcId = dto.PlcId,
            Device = dto.Device,
            MemoryCategory = dto.MemoryCategory,
            DeviceNumber = dto.DeviceNumber,
            DeviceNumber1 = dto.DeviceNumber1,
            DeviceNumber2 = dto.DeviceNumber2,
            Category = dto.Category,
            Row_1 = dto.Row_1,
            Row_2 = dto.Row_2,
            Row_3 = dto.Row_3,
            Row_4 = dto.Row_4,
            Direct_Input = dto.Direct_Input,
            Confirm = dto.Confirm,
            Note = dto.Note,
            GOT = dto.GOT,
            MnemonicId = dto.MnemonicId,
            RecordId = dto.RecordId,
            OutcoilNumber = dto.OutcoilNumber
        };

        public Memory ToDto() => new()
        {
            PlcId = this.PlcId,
            Device = this.Device,
            MemoryCategory = this.MemoryCategory,
            DeviceNumber = this.DeviceNumber,
            DeviceNumber1 = this.DeviceNumber1,
            DeviceNumber2 = this.DeviceNumber2,
            Category = this.Category,
            Row_1 = this.Row_1,
            Row_2 = this.Row_2,
            Row_3 = this.Row_3,
            Row_4 = this.Row_4,
            Direct_Input = this.Direct_Input,
            Confirm = this.Confirm,
            Note = this.Note,
            GOT = this.GOT,
            MnemonicId = this.MnemonicId,
            RecordId = this.RecordId,
            OutcoilNumber = this.OutcoilNumber
        };
    }
}
