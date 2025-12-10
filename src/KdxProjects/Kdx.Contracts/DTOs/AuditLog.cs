namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// データベースの変更履歴を記録するための監査ログ
    /// </summary>
    public class AuditLog
    {
        /// <summary>
        /// 監査ログID（自動生成）
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 変更されたテーブル名
        /// </summary>
        public string TableName { get; set; } = string.Empty;

        /// <summary>
        /// 変更されたレコードのID
        /// </summary>
        public string RecordId { get; set; } = string.Empty;

        /// <summary>
        /// 操作種別（INSERT, UPDATE, DELETE）
        /// </summary>
        public string Operation { get; set; } = string.Empty;

        /// <summary>
        /// 変更前のデータ（JSON形式）
        /// </summary>
        public string? OldData { get; set; }

        /// <summary>
        /// 変更後のデータ（JSON形式）
        /// </summary>
        public string? NewData { get; set; }

        /// <summary>
        /// 変更を行ったユーザーID
        /// </summary>
        public string? ChangedBy { get; set; }

        /// <summary>
        /// 変更を行ったユーザーのメールアドレス
        /// </summary>
        public string? ChangedByEmail { get; set; }

        /// <summary>
        /// 変更日時
        /// </summary>
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
            "Company" => "会社",
            "Model" => "機種",
            "PLC" => "PLC",
            "Cycle" => "サイクル",
            "Cylinder" => "シリンダー",
            "Operation" => "操作",
            "Process" => "工程",
            "ProcessDetail" => "工程詳細",
            "IO" => "IO",
            "Timer" => "タイマー",
            "Machine" => "機械",
            "MachineName" => "機械名称",
            "DriveMain" => "駆動部(主)",
            "DriveSub" => "駆動部(副)",
            "Interlock" => "インターロック",
            "InterlockCondition" => "インターロック条件",
            "Memory" => "メモリ",
            "MemoryProfile" => "メモリプロファイル",
            _ => TableName
        };
    }
}
