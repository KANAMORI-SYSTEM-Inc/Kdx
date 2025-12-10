using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("InterlockConditionType")]
    internal class InterlockConditionTypeEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ConditionTypeName")]
        public string? ConditionTypeName { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        [Column("NeedIOSearch")]
        public bool NeedIOSearch { get; set; }

        public static InterlockConditionTypeEntity FromDto(InterlockConditionType dto) => new()
        {
            Id = dto.Id,
            ConditionTypeName = dto.ConditionTypeName,
            Description = dto.Description,
            NeedIOSearch = dto.NeedIOSearch
        };

        public InterlockConditionType ToDto() => new()
        {
            Id = this.Id,
            ConditionTypeName = this.ConditionTypeName,
            Description = this.Description,
            NeedIOSearch = this.NeedIOSearch
        };
    }
}
