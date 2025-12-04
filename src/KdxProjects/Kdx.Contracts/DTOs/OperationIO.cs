namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// OperationとIOの関連付けを管理する中間テーブル
    /// </summary>
    public class OperationIO
    {
        public int OperationId { get; set; }

        public string IOAddress { get; set; } = string.Empty;

        // Alias for compatibility
        public string Address
        {
            get => IOAddress;
            set => IOAddress = value;
        }

        public int PlcId { get; set; } = 0;  // デフォルト値を0に設定

        /// <summary>
        /// IOの用途（Input/Output/Condition/Interlockなど）
        /// </summary>
        public string IOUsage { get; set; } = string.Empty;

        // Alias for compatibility
        public string Usage
        {
            get => IOUsage;
            set => IOUsage = value;
        }

        /// <summary>
        /// 表示順序
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// コメント
        /// </summary>
        public string? Comment { get; set; }
    }
}