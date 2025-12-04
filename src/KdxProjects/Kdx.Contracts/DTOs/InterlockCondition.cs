namespace Kdx.Contracts.DTOs
{
    public class InterlockCondition
    {
        // 複合主キー: (InterlockId, ConditionNumber, InterlockSortId)
        public int InterlockId { get; set; }

        public int ConditionNumber { get; set; }

        public int InterlockSortId { get; set; }

        public int? ConditionTypeId { get; set; }

        public string? Name { get; set; }

        public string? Device { get; set; }

        public bool? IsOnCondition { get; set; }
    }
}
