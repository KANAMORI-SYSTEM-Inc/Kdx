namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// 生成されたエラーメッセージの出力結果
    /// 複合キー: (PlcId, ErrorNum)
    /// PLCごとにエラー番号1~9999を割り当て
    /// </summary>
    public class GeneratedError
    {
        /// <summary>
        /// PLC ID (複合キーの一部)
        /// </summary>
        public int PlcId { get; set; }

        /// <summary>
        /// エラー番号 (複合キーの一部)
        /// PLCごとに1~9999の範囲
        /// </summary>
        public int ErrorNum { get; set; }

        /// <summary>
        /// 元のMnemonicType (Operation=3, CY=4, Interlock=6 など)
        /// </summary>
        public int MnemonicId { get; set; }

        /// <summary>
        /// 元のAlarmId
        /// </summary>
        public int AlarmId { get; set; }

        /// <summary>
        /// 元のレコードID (CylinderId, OperationId など)
        /// </summary>
        public int? RecordId { get; set; }

        /// <summary>
        /// Mデバイス (M1000 など)
        /// </summary>
        public string? DeviceM { get; set; }

        /// <summary>
        /// Tデバイス (T500 など)
        /// </summary>
        public string? DeviceT { get; set; }

        /// <summary>
        /// コメント1 (カテゴリ表示)
        /// </summary>
        public string? Comment1 { get; set; }

        /// <summary>
        /// コメント2
        /// </summary>
        public string? Comment2 { get; set; }

        /// <summary>
        /// コメント3
        /// </summary>
        public string? Comment3 { get; set; }

        /// <summary>
        /// コメント4
        /// </summary>
        public string? Comment4 { get; set; }

        /// <summary>
        /// アラームコメント
        /// </summary>
        public string? AlarmComment { get; set; }

        /// <summary>
        /// メッセージコメント
        /// </summary>
        public string? MessageComment { get; set; }

        /// <summary>
        /// エラー検出時間 (ms)
        /// </summary>
        public int ErrorTime { get; set; }

        /// <summary>
        /// Cycle ID (オプション)
        /// </summary>
        public int? CycleId { get; set; }
    }
}
