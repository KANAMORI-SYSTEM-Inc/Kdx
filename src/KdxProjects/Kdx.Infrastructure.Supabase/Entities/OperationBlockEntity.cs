using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("CY")]
    internal class OperationBlockEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("OperationBlockName")]
        public string? OperationBlockName { get; set; }

        public static OperationBlockEntity FromDto(OperationBlock dto) => new()
        {
            Id = dto.Id,
            OperationBlockName = dto.OperationBlockName
        };

        public OperationBlock ToDto() => new()
        {
            Id = this.Id,
            OperationBlockName = this.OperationBlockName
        };
    }
}
