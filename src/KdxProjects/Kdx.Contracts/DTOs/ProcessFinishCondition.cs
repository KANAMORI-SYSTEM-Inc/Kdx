namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// 工程の終了条件を管理する中間テーブル
    /// ProcessテーブルのFinishIdフィールド（複数値可）を正規化したもの
    /// 主キー: (ProcessId, FinishProcessDetailId)
    /// </summary>
    public class ProcessFinishCondition
    {
        public int? CycleId { get; set; }

        /// <summary>
        /// 工程ID (主キーの一部)
        /// </summary>
        public int ProcessId { get; set; }

        /// <summary>
        /// 終了条件となる工程詳細ID (主キーの一部)
        /// </summary>
        public int FinishProcessDetailId { get; set; }

        /// <summary>
        /// 終了センサー（オプション）
        /// </summary>
        public string? FinishSensor { get; set; }
    }
}
