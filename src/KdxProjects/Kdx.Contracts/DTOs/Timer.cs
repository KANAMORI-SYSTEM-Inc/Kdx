namespace Kdx.Contracts.DTOs
{
    public class Timer
    {
        public int ID { get; set; }
        public int? CycleId { get; set; }
        public int? TimerCategoryId { get; set; }
        public int? TimerNum { get; set; }
        public string? TimerName { get; set; }
        public int? MnemonicId { get; set; }
        public int? Example { get; set; }
    }
}
