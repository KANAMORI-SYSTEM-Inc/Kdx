namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// 出力エラー情報を表すDTO
    /// </summary>
    public class OutputError
    {
        /// <summary>
        /// エラーメッセージ
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// 対象ニモニックID
        /// </summary>
        public int? MnemonicId { get; set; }

        /// <summary>
        /// 対象レコードID
        /// </summary>
        public int? RecordId { get; set; }

        /// <summary>
        /// レコード名（工程名など）
        /// </summary>
        public string? RecordName { get; set; }

        /// <summary>
        /// 致命的なエラーかどうか
        /// </summary>
        public bool IsCritical { get; set; } = false;
    }
}