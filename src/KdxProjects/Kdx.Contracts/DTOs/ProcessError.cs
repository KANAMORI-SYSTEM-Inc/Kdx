namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// ProcessErrorテーブルのDTO
    /// エラー情報の中間データを保存し、これを元にラダープログラムとエラーメッセージを生成する
    /// </summary>
    public class ProcessError
    {
        public int? PlcId { get; set; }
        public int? CycleId { get; set; }
        public string? Device { get; set; }
        public int? MnemonicId { get; set; }
        public int? RecordId { get; set; }
        public int? AlarmId { get; set; }
        public int? AlarmCount { get; set; }

        /// <summary>
        /// 速度センサー番号（1=SS1, 2=SS2, 3=SS3, 4=SS4）
        /// AlarmId=3,4の場合に、どの速度センサーに対応するかを識別
        /// </summary>
        public int? SpeedNumber { get; set; }

        public int? ErrorNum { get; set; }
        public string? Comment1 { get; set; }
        public string? Comment2 { get; set; }
        public string? Comment3 { get; set; }
        public string? Comment4 { get; set; }
        public string? AlarmComment { get; set; }
        public string? MessageComment { get; set; }
        public int? ErrorTime { get; set; }
        public string? ErrorTimeDevice { get; set; }

        /// <summary>
        /// 要因となるIOアドレスのリスト（カンマ区切り）
        /// 例: "X100,X101,X102"
        /// </summary>
        public string? IoAddresses { get; set; }

        /// <summary>
        /// 要因となるIO名のリスト（カンマ区切り）
        /// 例: "G,B,SS1"
        /// </summary>
        public string? IoNames { get; set; }

        /// <summary>
        /// IO条件の説明（カンマ区切り）
        /// 例: "G:ON, B:OFF, SS1:ON"
        /// </summary>
        public string? IoConditions { get; set; }

        // Note: ErrorCountTime doesn't exist in the database table
        // This was likely a duplicate of ErrorTime in the original Access database


    }
}
