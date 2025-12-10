using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Kdx.Contracts.DTOs;

namespace Kdx.Infrastructure.Supabase.Entities
{
    /// <summary>
    /// インターロック前提条件3 Entity
    /// 特定IOまたは特定デバイスがONしている場合にインターロックが有効になる条件
    /// </summary>
    [Table("InterlockPrecondition3")]
    internal class InterlockPrecondition3Entity : BaseModel
    {
        [PrimaryKey("Id")]
        [Column("Id")]
        public int Id { get; set; }

        [Column("ConditionType")]
        public string? ConditionType { get; set; }

        [Column("IOAddress")]
        public string? IOAddress { get; set; }

        [Column("DeviceAddress")]
        public string? DeviceAddress { get; set; }

        [Column("IsOnCondition")]
        public bool IsOnCondition { get; set; }

        [Column("Description")]
        public string? Description { get; set; }

        public static InterlockPrecondition3Entity FromDto(InterlockPrecondition3 dto) => new()
        {
            Id = dto.Id,
            ConditionType = dto.ConditionType,
            IOAddress = dto.IOAddress,
            DeviceAddress = dto.DeviceAddress,
            IsOnCondition = dto.IsOnCondition,
            Description = dto.Description
        };

        public InterlockPrecondition3 ToDto() => new()
        {
            Id = this.Id,
            ConditionType = this.ConditionType,
            IOAddress = this.IOAddress,
            DeviceAddress = this.DeviceAddress,
            IsOnCondition = this.IsOnCondition,
            Description = this.Description
        };
    }
}
