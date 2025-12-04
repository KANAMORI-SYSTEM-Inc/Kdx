using Postgrest.Attributes;
using Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("Cylinder")]
    internal class CylinderEntity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("PUCO")]
        public string? PUCO { get; set; }

        [Column("CYNum")]
        public string CYNum { get; set; } = string.Empty;

        [Column("Go")]
        public string? Go { get; set; }

        [Column("Back")]
        public string? Back { get; set; }

        [Column("OilNum")]
        public string? OilNum { get; set; }

        [Column("MachineNameId")]
        public int? MachineNameId { get; set; }

        [Column("DriveSubId")]
        public int? DriveSubId { get; set; }

        [Column("PlaceId")]
        public int? PlaceId { get; set; }

        [Column("CYNameSub")]
        public string? CYNameSub { get; set; }

        [Column("SensorId")]
        public string? SensorId { get; set; }

        [Column("FlowType")]
        public string? FlowType { get; set; }

        [Column("GoSensorCount")]
        public string? GoSensorCount { get; set; }

        [Column("BackSensorCount")]
        public string? BackSensorCount { get; set; }

        [Column("RetentionSensorGo")]
        public string? RetentionSensorGo { get; set; }

        [Column("RetentionSensorBack")]
        public string? RetentionSensorBack { get; set; }

        [Column("SortNumber")]
        public int? SortNumber { get; set; }

        [Column("FlowCount")]
        public string? FlowCount { get; set; }

        [Column("FlowCYGo")]
        public string? FlowCYGo { get; set; }

        [Column("FlowCYBack")]
        public string? FlowCYBack { get; set; }

        public static CylinderEntity FromDto(Cylinder dto) => new()
        {
            Id = dto.Id,
            PlcId = dto.PlcId,
            PUCO = dto.PUCO,
            CYNum = dto.CYNum,
            Go = dto.Go,
            Back = dto.Back,
            OilNum = dto.OilNum,
            MachineNameId = dto.MachineNameId,
            DriveSubId = dto.DriveSubId,
            PlaceId = dto.PlaceId,
            CYNameSub = dto.CYNameSub,
            SensorId = dto.SensorId,
            FlowType = dto.FlowType,
            GoSensorCount = dto.GoSensorCount,
            BackSensorCount = dto.BackSensorCount,
            RetentionSensorGo = dto.RetentionSensorGo,
            RetentionSensorBack = dto.RetentionSensorBack,
            SortNumber = dto.SortNumber,
            FlowCount = dto.FlowCount,
            FlowCYGo = dto.FlowCYGo,
            FlowCYBack = dto.FlowCYBack
        };

        /// <summary>
        /// INSERT用のエンティティを作成（Idを除外して自動採番を有効化）
        /// </summary>
        public static CylinderEntityForInsert FromDtoForInsert(Cylinder dto) => new()
        {
            PlcId = dto.PlcId,
            PUCO = dto.PUCO,
            CYNum = dto.CYNum,
            Go = dto.Go,
            Back = dto.Back,
            OilNum = dto.OilNum,
            MachineNameId = dto.MachineNameId,
            DriveSubId = dto.DriveSubId,
            PlaceId = dto.PlaceId,
            CYNameSub = dto.CYNameSub,
            SensorId = dto.SensorId,
            FlowType = dto.FlowType,
            GoSensorCount = dto.GoSensorCount,
            BackSensorCount = dto.BackSensorCount,
            RetentionSensorGo = dto.RetentionSensorGo,
            RetentionSensorBack = dto.RetentionSensorBack,
            SortNumber = dto.SortNumber,
            FlowCount = dto.FlowCount,
            FlowCYGo = dto.FlowCYGo,
            FlowCYBack = dto.FlowCYBack
        };

        public Cylinder ToDto() => new()
        {
            Id = this.Id,
            PlcId = this.PlcId,
            PUCO = this.PUCO,
            CYNum = this.CYNum,
            Go = this.Go,
            Back = this.Back,
            OilNum = this.OilNum,
            MachineNameId = this.MachineNameId,
            DriveSubId = this.DriveSubId,
            PlaceId = this.PlaceId,
            CYNameSub = this.CYNameSub,
            SensorId = this.SensorId,
            FlowType = this.FlowType,
            GoSensorCount = this.GoSensorCount,
            BackSensorCount = this.BackSensorCount,
            RetentionSensorGo = this.RetentionSensorGo,
            RetentionSensorBack = this.RetentionSensorBack,
            SortNumber = this.SortNumber,
            FlowCount = this.FlowCount,
            FlowCYGo = this.FlowCYGo,
            FlowCYBack = this.FlowCYBack
        };
    }

    /// <summary>
    /// INSERT専用エンティティ（Idを含まない）
    /// </summary>
    [Table("Cylinder")]
    internal class CylinderEntityForInsert : BaseModel
    {
        [Column("PlcId")]
        public int PlcId { get; set; }

        [Column("PUCO")]
        public string? PUCO { get; set; }

        [Column("CYNum")]
        public string CYNum { get; set; } = string.Empty;

        [Column("Go")]
        public string? Go { get; set; }

        [Column("Back")]
        public string? Back { get; set; }

        [Column("OilNum")]
        public string? OilNum { get; set; }

        [Column("MachineNameId")]
        public int? MachineNameId { get; set; }

        [Column("DriveSubId")]
        public int? DriveSubId { get; set; }

        [Column("PlaceId")]
        public int? PlaceId { get; set; }

        [Column("CYNameSub")]
        public string? CYNameSub { get; set; }

        [Column("SensorId")]
        public string? SensorId { get; set; }

        [Column("FlowType")]
        public string? FlowType { get; set; }

        [Column("GoSensorCount")]
        public string? GoSensorCount { get; set; }

        [Column("BackSensorCount")]
        public string? BackSensorCount { get; set; }

        [Column("RetentionSensorGo")]
        public string? RetentionSensorGo { get; set; }

        [Column("RetentionSensorBack")]
        public string? RetentionSensorBack { get; set; }

        [Column("SortNumber")]
        public int? SortNumber { get; set; }

        [Column("FlowCount")]
        public string? FlowCount { get; set; }

        [Column("FlowCYGo")]
        public string? FlowCYGo { get; set; }

        [Column("FlowCYBack")]
        public string? FlowCYBack { get; set; }
    }
}
