using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Operation")]
    internal class OperationEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("OperationName")]
        public string? OperationName { get; set; }

        [Column("CYId")]
        public int? CYId { get; set; }

        [Column("CategoryId")]
        public int? CategoryId { get; set; }

        [Column("GoBack")]
        public string? GoBack { get; set; }

        [Column("Start")]
        public string? Start { get; set; }

        [Column("Finish")]
        public string? Finish { get; set; }

        [Column("Valve1")]
        public string? Valve1 { get; set; }

        [Column("S1")]
        public string? S1 { get; set; }

        [Column("S2")]
        public string? S2 { get; set; }

        [Column("S3")]
        public string? S3 { get; set; }

        [Column("S4")]
        public string? S4 { get; set; }

        [Column("S5")]
        public string? S5 { get; set; }

        [Column("SS1")]
        public string? SS1 { get; set; }

        [Column("SS2")]
        public string? SS2 { get; set; }

        [Column("SS3")]
        public string? SS3 { get; set; }

        [Column("SS4")]
        public string? SS4 { get; set; }

        [Column("PIL")]
        public string? PIL { get; set; }

        [Column("SC")]
        public string? SC { get; set; }

        [Column("FC")]
        public string? FC { get; set; }

        [Column("CycleId")]
        public int? CycleId { get; set; }

        [Column("SortNumber")]
        public int? SortNumber { get; set; }

        [Column("Con")]
        public string? Con { get; set; }

        public static OperationEntity FromDto(Operation dto) => new()
        {
            Id = dto.Id,
            OperationName = dto.OperationName,
            CYId = dto.CYId,
            CategoryId = dto.CategoryId,
            GoBack = dto.GoBack,
            Start = dto.Start,
            Finish = dto.Finish,
            Valve1 = dto.Valve1,
            S1 = dto.S1,
            S2 = dto.S2,
            S3 = dto.S3,
            S4 = dto.S4,
            S5 = dto.S5,
            SS1 = dto.SS1,
            SS2 = dto.SS2,
            SS3 = dto.SS3,
            SS4 = dto.SS4,
            PIL = dto.PIL,
            SC = dto.SC,
            FC = dto.FC,
            CycleId = dto.CycleId,
            SortNumber = dto.SortNumber,
            Con = dto.Con
        };

        /// <summary>
        /// INSERT用のエンティティを作成（Idを除外して自動採番を有効化）
        /// </summary>
        public static OperationEntityForInsert FromDtoForInsert(Operation dto) => new()
        {
            OperationName = dto.OperationName,
            CYId = dto.CYId,
            CategoryId = dto.CategoryId,
            GoBack = dto.GoBack,
            Start = dto.Start,
            Finish = dto.Finish,
            Valve1 = dto.Valve1,
            S1 = dto.S1,
            S2 = dto.S2,
            S3 = dto.S3,
            S4 = dto.S4,
            S5 = dto.S5,
            SS1 = dto.SS1,
            SS2 = dto.SS2,
            SS3 = dto.SS3,
            SS4 = dto.SS4,
            PIL = dto.PIL,
            SC = dto.SC,
            FC = dto.FC,
            CycleId = dto.CycleId,
            SortNumber = dto.SortNumber,
            Con = dto.Con
        };

        public Operation ToDto() => new()
        {
            Id = this.Id,
            OperationName = this.OperationName,
            CYId = this.CYId,
            CategoryId = this.CategoryId,
            GoBack = this.GoBack,
            Start = this.Start,
            Finish = this.Finish,
            Valve1 = this.Valve1,
            S1 = this.S1,
            S2 = this.S2,
            S3 = this.S3,
            S4 = this.S4,
            S5 = this.S5,
            SS1 = this.SS1,
            SS2 = this.SS2,
            SS3 = this.SS3,
            SS4 = this.SS4,
            PIL = this.PIL,
            SC = this.SC,
            FC = this.FC,
            CycleId = this.CycleId,
            SortNumber = this.SortNumber,
            Con = this.Con
        };
    }

    /// <summary>
    /// INSERT専用エンティティ（Idを含まない）
    /// </summary>
    [Table("Operation")]
    internal class OperationEntityForInsert : BaseModel
    {
        [Column("OperationName")]
        public string? OperationName { get; set; }

        [Column("CYId")]
        public int? CYId { get; set; }

        [Column("CategoryId")]
        public int? CategoryId { get; set; }

        [Column("GoBack")]
        public string? GoBack { get; set; }

        [Column("Start")]
        public string? Start { get; set; }

        [Column("Finish")]
        public string? Finish { get; set; }

        [Column("Valve1")]
        public string? Valve1 { get; set; }

        [Column("S1")]
        public string? S1 { get; set; }

        [Column("S2")]
        public string? S2 { get; set; }

        [Column("S3")]
        public string? S3 { get; set; }

        [Column("S4")]
        public string? S4 { get; set; }

        [Column("S5")]
        public string? S5 { get; set; }

        [Column("SS1")]
        public string? SS1 { get; set; }

        [Column("SS2")]
        public string? SS2 { get; set; }

        [Column("SS3")]
        public string? SS3 { get; set; }

        [Column("SS4")]
        public string? SS4 { get; set; }

        [Column("PIL")]
        public string? PIL { get; set; }

        [Column("SC")]
        public string? SC { get; set; }

        [Column("FC")]
        public string? FC { get; set; }

        [Column("CycleId")]
        public int? CycleId { get; set; }

        [Column("SortNumber")]
        public int? SortNumber { get; set; }

        [Column("Con")]
        public string? Con { get; set; }
    }
}
