using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("InterlockPrecondition1")]
    internal class InterlockPrecondition1Entity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("Discription")]
        public string? Discription { get; set; }

        [Column("ConditionName")]
        public string? ConditionName { get; set; }

        public static InterlockPrecondition1Entity FromDto(InterlockPrecondition1 dto) => new()
        {
            Id = dto.Id,
            Discription = dto.Discription,
            ConditionName = dto.ConditionName
        };

        public InterlockPrecondition1 ToDto() => new()
        {
            Id = this.Id,
            Discription = this.Discription,
            ConditionName = this.ConditionName
        };
    }
}
