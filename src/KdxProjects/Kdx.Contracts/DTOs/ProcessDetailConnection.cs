namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// ProcessDetail間の接続を表すDTO
    /// 主キー: (FromProcessDetailId, ToProcessDetailId)の複合キー
    /// </summary>
    public class ProcessDetailConnection
    {
        /// <summary>
        /// 接続元のProcessDetail ID（複合主キーの一部）
        /// </summary>
        public int FromProcessDetailId { get; set; }

        /// <summary>
        /// 接続先のProcessDetail ID（複合主キーの一部）
        /// </summary>
        public int? ToProcessDetailId { get; set; }

        /// <summary>
        /// サイクルID
        /// 接続が所属するサイクルを識別します
        /// </summary>
        public int? CycleId { get; set; }
    }
}