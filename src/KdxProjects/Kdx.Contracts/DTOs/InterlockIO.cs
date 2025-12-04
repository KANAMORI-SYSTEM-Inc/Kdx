namespace Kdx.Contracts.DTOs
{
    public class InterlockIO
    {
        // 複合主キー: (InterlockId, PlcId, IOAddress, InterlockSortId, ConditionNumber)
        public int InterlockId { get; set; }

        public int PlcId { get; set; }

        public string IOAddress { get; set; } = string.Empty;

        public int InterlockSortId { get; set; }

        public int ConditionNumber { get; set; }

        public bool IsOnCondition { get; set; } = false;
    }
}
