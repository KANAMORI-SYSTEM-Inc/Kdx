using Supabase.Postgrest.Attributes;
using Supabase.Postgrest.Models;
using Kdx.Contracts.DTOs;
using System.Text.Json;

namespace Kdx.Infrastructure.Supabase.Entities
{
    [Table("audit_log")]
    internal class AuditLogEntity : BaseModel
    {
        [PrimaryKey("id")]
        [Column("id")]
        public long Id { get; set; }

        [Column("table_name")]
        public new string TableName { get; set; } = string.Empty;

        [Column("record_id")]
        public string RecordId { get; set; } = string.Empty;

        [Column("operation")]
        public string Operation { get; set; } = string.Empty;

        [Column("old_data")]
        public object? OldData { get; set; }

        [Column("new_data")]
        public object? NewData { get; set; }

        [Column("changed_by")]
        public string? ChangedBy { get; set; }

        [Column("changed_at")]
        public DateTime ChangedAt { get; set; }

        public AuditLog ToDto() => new()
        {
            Id = this.Id,
            TableName = this.TableName,
            RecordId = this.RecordId,
            Operation = this.Operation,
            OldData = ConvertToJsonString(this.OldData),
            NewData = ConvertToJsonString(this.NewData),
            ChangedBy = this.ChangedBy,
            ChangedAt = this.ChangedAt
        };

        protected static string? ConvertToJsonString(object? data)
        {
            if (data == null) return null;
            if (data is string str) return str;
            if (data is JsonElement jsonElement) return jsonElement.GetRawText();
            return JsonSerializer.Serialize(data);
        }
    }

    /// <summary>
    /// audit_log_with_user ビュー用のエンティティ（メールアドレス付き）
    /// </summary>
    [Table("audit_log_with_user")]
    internal class AuditLogWithUserEntity : BaseModel
    {
        [PrimaryKey("id")]
        [Column("id")]
        public long Id { get; set; }

        [Column("table_name")]
        public new string TableName { get; set; } = string.Empty;

        [Column("record_id")]
        public string RecordId { get; set; } = string.Empty;

        [Column("operation")]
        public string Operation { get; set; } = string.Empty;

        [Column("old_data")]
        public object? OldData { get; set; }

        [Column("new_data")]
        public object? NewData { get; set; }

        [Column("changed_by")]
        public string? ChangedBy { get; set; }

        [Column("changed_by_email")]
        public string? ChangedByEmail { get; set; }

        [Column("changed_at")]
        public DateTime ChangedAt { get; set; }

        public AuditLog ToDto() => new()
        {
            Id = this.Id,
            TableName = this.TableName,
            RecordId = this.RecordId,
            Operation = this.Operation,
            OldData = ConvertToJsonString(this.OldData),
            NewData = ConvertToJsonString(this.NewData),
            ChangedBy = this.ChangedBy,
            ChangedByEmail = this.ChangedByEmail,
            ChangedAt = this.ChangedAt
        };

        private static string? ConvertToJsonString(object? data)
        {
            if (data == null) return null;
            if (data is string str) return str;
            if (data is JsonElement jsonElement) return jsonElement.GetRawText();
            return JsonSerializer.Serialize(data);
        }
    }
}
