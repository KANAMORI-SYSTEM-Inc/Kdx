using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Definitions")]
    internal class DefinitionsEntity : BaseModel
    {
        [PrimaryKey("ID")]
        [Column("ID")]
        public int ID { get; set; }

        [Column("DefName")]
        public string DefName { get; set; } = string.Empty;

        [Column("OutCoilNumber")]
        public int OutCoilNumber { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("Comment1")]
        public string? Comment1 { get; set; }

        [Column("Comment2")]
        public string? Comment2 { get; set; }

        [Column("Label")]
        public string Label { get; set; } = "M";

        [Column("Category")]
        public string Category { get; set; } = "Nan";

        [Column("MnemonicId")]
        public int MnemonicId { get; set; }

        [Column("MemoryCategoryId")]
        public int MemoryCategoryId { get; set; }

        public static DefinitionsEntity FromDto(Definitions dto) => new()
        {
            ID = dto.ID,
            DefName = dto.DefName,
            OutCoilNumber = dto.OutCoilNumber,
            Description = dto.Description,
            Comment1 = dto.Comment1,
            Comment2 = dto.Comment2,
            Label = dto.Label,
            Category = dto.Category,
            MnemonicId = dto.MnemonicId,
            MemoryCategoryId = dto.MemoryCategoryId
        };

        public Definitions ToDto() => new()
        {
            ID = this.ID,
            DefName = this.DefName,
            OutCoilNumber = this.OutCoilNumber,
            Description = this.Description,
            Comment1 = this.Comment1,
            Comment2 = this.Comment2,
            Label = this.Label,
            Category = this.Category,
            MnemonicId = this.MnemonicId,
            MemoryCategoryId = this.MemoryCategoryId
        };
    }
}
