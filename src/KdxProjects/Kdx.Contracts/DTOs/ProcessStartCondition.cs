namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// 工程の開始条件を管理する中間テーブル
    /// ProcessテーブルのAutoConditionフィールド（複数値）を正規化したもの
    /// 主キー: (ProcessId, StartProcessDetailId)
    /// </summary>
    public class ProcessStartCondition
    {
        public int? CycleId { get; set; }

        /// <summary>
        /// 工程ID (主キーの一部)
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// 開始条件となる工程詳細ID (主キーの一部)
        /// </summary>
        public int StartProcessDetailId { get; set; }

        /// <summary>
        /// 開始センサー（オプション）
        /// </summary>
        public string? StartSensor { get; set; }
    }
}