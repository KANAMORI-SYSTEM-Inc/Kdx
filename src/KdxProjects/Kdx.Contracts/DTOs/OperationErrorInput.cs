namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// Operationエラーメッセージ生成用の入力データ
    /// メモリストアから取得したOperation関連のMemoryデータをこの形式に変換して使用
    /// </summary>
    public class OperationErrorInput
    {
        /// <summary>
        /// Operation ID
        /// </summary>
        public int OperationId { get; set; }

        /// <summary>
        /// Operation名
        /// </summary>
        public string? OperationName { get; set; }

        /// <summary>
        /// カテゴリID
        /// </summary>
        public int? CategoryId { get; set; }

        /// <summary>
        /// カテゴリ名
        /// </summary>
        public string? CategoryName { get; set; }

        /// <summary>
        /// バルブ1
        /// </summary>
        public string? Valve1 { get; set; }

        /// <summary>
        /// バルブ2
        /// </summary>
        public string? Valve2 { get; set; }

        /// <summary>
        /// GoBack
        /// </summary>
        public string? GoBack { get; set; }

        /// <summary>
        /// 入力デバイス
        /// </summary>
        public string? InputDevice { get; set; }

        /// <summary>
        /// 出力デバイス
        /// </summary>
        public string? OutputDevice { get; set; }

        /// <summary>
        /// 割り当て済みデバイス
        /// </summary>
        public string? Device { get; set; }

        /// <summary>
        /// デバイス番号
        /// </summary>
        public int DeviceNumber { get; set; }

        /// <summary>
        /// PLC ID
        /// </summary>
        public int PlcId { get; set; }

        /// <summary>
        /// Cycle ID
        /// </summary>
        public int CycleId { get; set; }
    }
}
