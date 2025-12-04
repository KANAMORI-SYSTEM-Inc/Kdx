namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// CYテーブルとIOテーブルを関連付ける中間テーブル
    /// </summary>
    public class CylinderIO
    {
        public int CylinderId { get; set; }  // CYテーブルのId

        public string IOAddress { get; set; } = string.Empty;  // IOテーブルのAddress

        // Alias for compatibility
        public string Address
        {
            get => IOAddress;
            set => IOAddress = value;
        }

        public int PlcId { get; set; } = 0;  // IOテーブルのPlcId（複合キーの一部）

        /// <summary>
        /// IOの用途タイプ
        /// </summary>
        public string IOType { get; set; } = string.Empty;  // "GoSensor", "BackSensor", "GoValve", "BackValve" など

        /// <summary>
        /// 表示順序
        /// </summary>
        public int? SortOrder { get; set; }

        /// <summary>
        /// 備考
        /// </summary>
        public string? Comment { get; set; }
    }
}