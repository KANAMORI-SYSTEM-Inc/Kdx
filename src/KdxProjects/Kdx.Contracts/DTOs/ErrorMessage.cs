namespace Kdx.Contracts.DTOs
{
    public class ErrorMessage
    {
        public int MnemonicId { get; set; }
        public int AlarmId { get; set; }
        public string? BaseMessage { get; set; }
        public string? BaseAlarm { get; set; }
        public string? Category1 { get; set; }
        public string? Category2 { get; set; }
        public string? Category3 { get; set; }

        public int DefaultCountTime { get; set; } = 0;
    }
}