using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("TimerCategory")]
    internal class TimerCategoryEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("CategoryName")]
        public string? CategoryName { get; set; }

        [Column("CategoryFig")]
        public string? CategoryFig { get; set; }

        public static TimerCategoryEntity FromDto(TimerCategory dto) => new()
        {
            Id = dto.Id,
            CategoryName = dto.CategoryName,
            CategoryFig = dto.CategoryFig
        };

        public TimerCategory ToDto() => new()
        {
            Id = this.Id,
            CategoryName = this.CategoryName,
            CategoryFig = this.CategoryFig
        };
    }
}
