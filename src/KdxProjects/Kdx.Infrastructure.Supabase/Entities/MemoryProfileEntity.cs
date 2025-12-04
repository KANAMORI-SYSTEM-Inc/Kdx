using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    /// <summary>
    /// MemoryProfileテーブルのエンティティ
    /// CycleIdが主キー
    /// </summary>
    [Table("MemoryProfile")]
    public class MemoryProfileEntity : BaseModel
    {
        [PrimaryKey("CycleId", false)]
        [Column("CycleId")]
        public int CycleId { get; set; }

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Description")]
        public string? Description { get; set; }

        [Column("IsDefault")]
        public bool IsDefault { get; set; }

        // デバイス開始アドレス設定
        [Column("ProcessDeviceStartL")]
        public int ProcessDeviceStartL { get; set; }

        [Column("DetailDeviceStartL")]
        public int DetailDeviceStartL { get; set; }

        [Column("OperationDeviceStartM")]
        public int OperationDeviceStartM { get; set; }

        [Column("CylinderDeviceStartM")]
        public int CylinderDeviceStartM { get; set; }

        [Column("CylinderDeviceStartD")]
        public int CylinderDeviceStartD { get; set; }

        [Column("ErrorDeviceStartM")]
        public int ErrorDeviceStartM { get; set; }

        [Column("ErrorDeviceStartT")]
        public int ErrorDeviceStartT { get; set; }

        [Column("DeviceStartT")]
        public int DeviceStartT { get; set; }

        [Column("TimerStartZR")]
        public int TimerStartZR { get; set; }

        [Column("ProsTimeStartZR")]
        public int ProsTimeStartZR { get; set; }

        [Column("ProsTimePreviousStartZR")]
        public int ProsTimePreviousStartZR { get; set; }

        [Column("CyTimeStartZR")]
        public int CyTimeStartZR { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; }

        [Column("UpdatedAt")]
        public DateTime UpdatedAt { get; set; }

        /// <summary>
        /// DTOからエンティティを作成
        /// </summary>
        public static MemoryProfileEntity FromDto(MemoryProfile dto)
        {
            return new MemoryProfileEntity
            {
                CycleId = dto.CycleId,
                Name = dto.Name,
                Description = dto.Description,
                IsDefault = dto.IsDefault,
                ProcessDeviceStartL = dto.ProcessDeviceStartL,
                DetailDeviceStartL = dto.DetailDeviceStartL,
                OperationDeviceStartM = dto.OperationDeviceStartM,
                CylinderDeviceStartM = dto.CylinderDeviceStartM,
                CylinderDeviceStartD = dto.CylinderDeviceStartD,
                ErrorDeviceStartM = dto.ErrorDeviceStartM,
                ErrorDeviceStartT = dto.ErrorDeviceStartT,
                DeviceStartT = dto.DeviceStartT,
                TimerStartZR = dto.TimerStartZR,
                ProsTimeStartZR = dto.ProsTimeStartZR,
                ProsTimePreviousStartZR = dto.ProsTimePreviousStartZR,
                CyTimeStartZR = dto.CyTimeStartZR,
                CreatedAt = dto.CreatedAt,
                UpdatedAt = dto.UpdatedAt
            };
        }

        /// <summary>
        /// エンティティをDTOに変換
        /// </summary>
        public MemoryProfile ToDto()
        {
            return new MemoryProfile
            {
                CycleId = this.CycleId,
                Name = this.Name,
                Description = this.Description,
                IsDefault = this.IsDefault,
                ProcessDeviceStartL = this.ProcessDeviceStartL,
                DetailDeviceStartL = this.DetailDeviceStartL,
                OperationDeviceStartM = this.OperationDeviceStartM,
                CylinderDeviceStartM = this.CylinderDeviceStartM,
                CylinderDeviceStartD = this.CylinderDeviceStartD,
                ErrorDeviceStartM = this.ErrorDeviceStartM,
                ErrorDeviceStartT = this.ErrorDeviceStartT,
                DeviceStartT = this.DeviceStartT,
                TimerStartZR = this.TimerStartZR,
                ProsTimeStartZR = this.ProsTimeStartZR,
                ProsTimePreviousStartZR = this.ProsTimePreviousStartZR,
                CyTimeStartZR = this.CyTimeStartZR,
                CreatedAt = this.CreatedAt,
                UpdatedAt = this.UpdatedAt
            };
        }

        /// <summary>
        /// DTOから挿入用エンティティを作成
        /// </summary>
        public static MemoryProfileEntityForInsert FromDtoForInsert(MemoryProfile dto)
        {
            return new MemoryProfileEntityForInsert
            {
                CycleId = dto.CycleId,
                Name = dto.Name,
                Description = dto.Description,
                IsDefault = dto.IsDefault,
                ProcessDeviceStartL = dto.ProcessDeviceStartL,
                DetailDeviceStartL = dto.DetailDeviceStartL,
                OperationDeviceStartM = dto.OperationDeviceStartM,
                CylinderDeviceStartM = dto.CylinderDeviceStartM,
                CylinderDeviceStartD = dto.CylinderDeviceStartD,
                ErrorDeviceStartM = dto.ErrorDeviceStartM,
                ErrorDeviceStartT = dto.ErrorDeviceStartT,
                DeviceStartT = dto.DeviceStartT,
                TimerStartZR = dto.TimerStartZR,
                ProsTimeStartZR = dto.ProsTimeStartZR,
                ProsTimePreviousStartZR = dto.ProsTimePreviousStartZR,
                CyTimeStartZR = dto.CyTimeStartZR
            };
        }
    }

    /// <summary>
    /// MemoryProfile挿入用エンティティ（CreatedAt/UpdatedAtは自動生成）
    /// CycleIdが主キー
    /// </summary>
    [Table("MemoryProfile")]
    public class MemoryProfileEntityForInsert : BaseModel
    {
        [PrimaryKey("CycleId", false)]
        [Column("CycleId")]
        public int CycleId { get; set; }

        [Column("Name")]
        public string Name { get; set; } = string.Empty;

        [Column("Description")]
        public string? Description { get; set; }

        [Column("IsDefault")]
        public bool IsDefault { get; set; }

        [Column("ProcessDeviceStartL")]
        public int ProcessDeviceStartL { get; set; }

        [Column("DetailDeviceStartL")]
        public int DetailDeviceStartL { get; set; }

        [Column("OperationDeviceStartM")]
        public int OperationDeviceStartM { get; set; }

        [Column("CylinderDeviceStartM")]
        public int CylinderDeviceStartM { get; set; }

        [Column("CylinderDeviceStartD")]
        public int CylinderDeviceStartD { get; set; }

        [Column("ErrorDeviceStartM")]
        public int ErrorDeviceStartM { get; set; }

        [Column("ErrorDeviceStartT")]
        public int ErrorDeviceStartT { get; set; }

        [Column("DeviceStartT")]
        public int DeviceStartT { get; set; }

        [Column("TimerStartZR")]
        public int TimerStartZR { get; set; }

        [Column("ProsTimeStartZR")]
        public int ProsTimeStartZR { get; set; }

        [Column("ProsTimePreviousStartZR")]
        public int ProsTimePreviousStartZR { get; set; }

        [Column("CyTimeStartZR")]
        public int CyTimeStartZR { get; set; }
    }
}
