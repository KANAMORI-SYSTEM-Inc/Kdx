using Postgrest.Attributes;

namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// データベースの変更履歴を記録するための監査ログ
    /// </summary>
    [Table("audit_log")]
    public class AuditLog
    {
        /// <summary>
        /// 監査ログID（自動生成）
        /// </summary>
        [PrimaryKey("id")]
        [Column("id")]
        public long Id { get; set; }

        /// <summary>
        /// 変更されたテーブル名
        /// </summary>
        [Column("table_name")]
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// 変更されたレコードのID
        /// </summary>
        [Column("record_id")]
        public string RecordId { get; set; } = string.Empty;

        /// <summary>
        /// 操作種別（INSERT, UPDATE, DELETE）
        /// </summary>
        [Column("operation")]
        public string Operation { get; set; } = string.Empty;

        /// <summary>
        /// 変更前のデータ（JSON形式）
        /// </summary>
        [Column("old_data")]
        public string? OldData { get; set; }

        /// <summary>
        /// 変更後のデータ（JSON形式）
        /// </summary>
        [Column("new_data")]
        public string? NewData { get; set; }

        /// <summary>
        /// 変更を行ったユーザーID
        /// </summary>
        [Column("changed_by")]
        public string? ChangedBy { get; set; }

        /// <summary>
        /// 変更日時
        /// </summary>
        [Column("changed_at")]
        public DateTime ChangedAt { get; set; }

        /// <summary>
        /// 操作種別の日本語表示
        /// </summary>
        public string OperationDisplayName => Operation switch
        {
            "INSERT" => "追加",
            "UPDATE" => "更新",
            "DELETE" => "削除",
            _ => Operation
        };

        /// <summary>
        /// テーブル名の日本語表示
        /// </summary>
        public string TableDisplayName => TableName switch
        {
            "companies" => "会社",
            "models" => "機種",
            "plcs" => "PLC",
            "cycles" => "サイクル",
            "cylinders" => "シリンダー",
            "operations" => "操作",
            "processes" => "工程",
            "process_details" => "工程詳細",
            "ios" => "IO",
            "timers" => "タイマー",
            "machines" => "機械",
            "machine_names" => "機械名称",
            "drive_mains" => "駆動部(主)",
            "drive_subs" => "駆動部(副)",
            "interlocks" => "インターロック",
            "interlock_conditions" => "インターロック条件",
            "memories" => "メモリ",
            "memory_profiles" => "メモリプロファイル",
            _ => TableName
        };
    }
}
