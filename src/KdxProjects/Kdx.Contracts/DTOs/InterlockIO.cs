namespace Kdx.Contracts.DTOs
{
    public class InterlockIO
    {
        // 複合主キー: (CylinderId, PlcId, IOAddress, InterlockSortId, ConditionNumber)
        public int CylinderId { get; set; }

        public int PlcId { get; set; }

        public string IOAddress { get; set; } = string.Empty;

        public int InterlockSortId { get; set; }

        public int ConditionNumber { get; set; }

        public bool IsOnCondition { get; set; } = false;

        /// <summary>
        /// IO番号/インデックス (0から始まる)
        /// 同一InterlockCondition内での順序を示す
        /// </summary>
        public int IoIndex { get; set; } = 0;
    }
}
