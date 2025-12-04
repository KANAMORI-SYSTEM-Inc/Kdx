namespace Kdx.Contracts.DTOs
{
    public class InterlockConditionType
    {
        public int Id { get; set; }

        public string? ConditionTypeName { get; set; }

        public string? Discription { get; set; }

        public bool NeedIOSearch { get; set; } = false;

    }
}
