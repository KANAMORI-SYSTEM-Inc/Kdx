using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("ProcessDetail")]
    internal class ProcessDetailEntity : BaseModel
    {
        [PrimaryKey("ID")]
        [Column("ID")]
        public int Id { get; set; }

        [Column("ProcessId")]
        public int ProcessId { get; set; }

        [Column("OperationId")]
        public int? OperationId { get; set; }

        [Column("DetailName")]
        public string? DetailName { get; set; }

        [Column("StartSensor")]
        public string? StartSensor { get; set; }

        [Column("CategoryId")]
        public int? CategoryId { get; set; }

        [Column("FinishSensor")]
        public string? FinishSensor { get; set; }

        [Column("BlockNumber")]
        public int? BlockNumber { get; set; }

        [Column("SkipMode")]
        public string? SkipMode { get; set; }

        [Column("CycleId")]
        public int? CycleId { get; set; }

        [Column("SortNumber")]
        public int? SortNumber { get; set; }

        [Column("Comment")]
        public string? Comment { get; set; }

        [Column("ILStart")]
        public string? ILStart { get; set; }

        [Column("StartTimerId")]
        public int? StartTimerId { get; set; }

        [Column("IsResetAfter")]
        public bool? IsResetAfter { get; set; }

        public static ProcessDetailEntity FromDto(ProcessDetail dto) => new()
        {
            Id = dto.Id,
            ProcessId = dto.ProcessId,
            OperationId = dto.OperationId,
            DetailName = dto.DetailName,
            StartSensor = dto.StartSensor,
            CategoryId = dto.CategoryId,
            FinishSensor = dto.FinishSensor,
            BlockNumber = dto.BlockNumber,
            SkipMode = dto.SkipMode,
            CycleId = dto.CycleId,
            SortNumber = dto.SortNumber,
            Comment = dto.Comment,
            ILStart = dto.ILStart,
            StartTimerId = dto.StartTimerId,
            IsResetAfter = dto.IsResetAfter
        };

        /// <summary>
        /// INSERT用のエンティティを作成（Idを除外して自動採番を有効化）
        /// </summary>
        public static ProcessDetailEntityForInsert FromDtoForInsert(ProcessDetail dto) => new()
        {
            ProcessId = dto.ProcessId,
            OperationId = dto.OperationId,
            DetailName = dto.DetailName,
            StartSensor = dto.StartSensor,
            CategoryId = dto.CategoryId,
            FinishSensor = dto.FinishSensor,
            BlockNumber = dto.BlockNumber,
            SkipMode = dto.SkipMode,
            CycleId = dto.CycleId,
            SortNumber = dto.SortNumber,
            Comment = dto.Comment,
            ILStart = dto.ILStart,
            StartTimerId = dto.StartTimerId,
            IsResetAfter = dto.IsResetAfter
        };

        public ProcessDetail ToDto() => new()
        {
            Id = this.Id,
            ProcessId = this.ProcessId,
            OperationId = this.OperationId,
            DetailName = this.DetailName,
            StartSensor = this.StartSensor,
            CategoryId = this.CategoryId,
            FinishSensor = this.FinishSensor,
            BlockNumber = this.BlockNumber,
            SkipMode = this.SkipMode,
            CycleId = this.CycleId,
            SortNumber = this.SortNumber,
            Comment = this.Comment,
            ILStart = this.ILStart,
            StartTimerId = this.StartTimerId,
            IsResetAfter = this.IsResetAfter
        };
    }

    /// <summary>
    /// INSERT専用エンティティ（Idを含まない）
    /// </summary>
    [Table("ProcessDetail")]
    internal class ProcessDetailEntityForInsert : BaseModel
    {
        [Column("ProcessId")]
        public int ProcessId { get; set; }

        [Column("OperationId")]
        public int? OperationId { get; set; }

        [Column("DetailName")]
        public string? DetailName { get; set; }

        [Column("StartSensor")]
        public string? StartSensor { get; set; }

        [Column("CategoryId")]
        public int? CategoryId { get; set; }

        [Column("FinishSensor")]
        public string? FinishSensor { get; set; }

        [Column("BlockNumber")]
        public int? BlockNumber { get; set; }

        [Column("SkipMode")]
        public string? SkipMode { get; set; }

        [Column("CycleId")]
        public int? CycleId { get; set; }

        [Column("SortNumber")]
        public int? SortNumber { get; set; }

        [Column("Comment")]
        public string? Comment { get; set; }

        [Column("ILStart")]
        public string? ILStart { get; set; }

        [Column("StartTimerId")]
        public int? StartTimerId { get; set; }

        [Column("IsResetAfter")]
        public bool? IsResetAfter { get; set; }
    }
}
