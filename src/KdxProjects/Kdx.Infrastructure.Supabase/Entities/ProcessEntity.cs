using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Process")]
    internal class ProcessEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ProcessName")]
        public string? ProcessName { get; set; }

        [Column("CycleId")]
        public int? CycleId { get; set; }

        [Column("TestStart")]
        public string? TestStart { get; set; }

        [Column("TestCondition")]
        public string? TestCondition { get; set; }

        [Column("TestMode")]
        public string? TestMode { get; set; }

        [Column("AutoMode")]
        public string? AutoMode { get; set; }

        [Column("AutoStart")]
        public string? AutoStart { get; set; }

        [Column("ProcessCategoryId")]
        public int? ProcessCategoryId { get; set; }

        [Column("ILStart")]
        public string? ILStart { get; set; }

        [Column("Comment1")]
        public string? Comment1 { get; set; }

        [Column("Comment2")]
        public string? Comment2 { get; set; }

        [Column("SortNumber")]
        public int? SortNumber { get; set; }

        public static ProcessEntity FromDto(Process dto) => new()
        {
            Id = dto.Id,
            ProcessName = dto.ProcessName,
            CycleId = dto.CycleId,
            TestStart = dto.TestStart,
            TestCondition = dto.TestCondition,
            TestMode = dto.TestMode,
            AutoMode = dto.AutoMode,
            AutoStart = dto.AutoStart,
            ProcessCategoryId = dto.ProcessCategoryId,
            ILStart = dto.ILStart,
            Comment1 = dto.Comment1,
            Comment2 = dto.Comment2,
            SortNumber = dto.SortNumber
        };

        /// <summary>
        /// INSERT用のエンティティを作成（Idを除外して自動採番を有効化）
        /// </summary>
        public static ProcessEntityForInsert FromDtoForInsert(Process dto) => new()
        {
            ProcessName = dto.ProcessName,
            CycleId = dto.CycleId,
            TestStart = dto.TestStart,
            TestCondition = dto.TestCondition,
            TestMode = dto.TestMode,
            AutoMode = dto.AutoMode,
            AutoStart = dto.AutoStart,
            ProcessCategoryId = dto.ProcessCategoryId,
            ILStart = dto.ILStart,
            Comment1 = dto.Comment1,
            Comment2 = dto.Comment2,
            SortNumber = dto.SortNumber
        };

        public Process ToDto() => new()
        {
            Id = this.Id,
            ProcessName = this.ProcessName,
            CycleId = this.CycleId,
            TestStart = this.TestStart,
            TestCondition = this.TestCondition,
            TestMode = this.TestMode,
            AutoMode = this.AutoMode,
            AutoStart = this.AutoStart,
            ProcessCategoryId = this.ProcessCategoryId,
            ILStart = this.ILStart,
            Comment1 = this.Comment1,
            Comment2 = this.Comment2,
            SortNumber = this.SortNumber
        };
    }

    /// <summary>
    /// INSERT専用エンティティ（Idを含まない）
    /// </summary>
    [Table("Process")]
    internal class ProcessEntityForInsert : BaseModel
    {
        [Column("ProcessName")]
        public string? ProcessName { get; set; }

        [Column("CycleId")]
        public int? CycleId { get; set; }

        [Column("TestStart")]
        public string? TestStart { get; set; }

        [Column("TestCondition")]
        public string? TestCondition { get; set; }

        [Column("TestMode")]
        public string? TestMode { get; set; }

        [Column("AutoMode")]
        public string? AutoMode { get; set; }

        [Column("AutoStart")]
        public string? AutoStart { get; set; }

        [Column("ProcessCategoryId")]
        public int? ProcessCategoryId { get; set; }

        [Column("ILStart")]
        public string? ILStart { get; set; }

        [Column("Comment1")]
        public string? Comment1 { get; set; }

        [Column("Comment2")]
        public string? Comment2 { get; set; }

        [Column("SortNumber")]
        public int? SortNumber { get; set; }
    }
}
