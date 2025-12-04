using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Place")]
    internal class PlaceEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ModelId")]
        public int? ModelId { get; set; }

        [Column("PlaceName")]
        public string? PlaceName { get; set; }

        public static PlaceEntity FromDto(Place dto) => new()
        {
            Id = dto.Id,
            ModelId = dto.ModelId,
            PlaceName = dto.PlaceName
        };

        public Place ToDto() => new()
        {
            Id = this.Id,
            ModelId = this.ModelId,
            PlaceName = this.PlaceName
        };
    }
}
