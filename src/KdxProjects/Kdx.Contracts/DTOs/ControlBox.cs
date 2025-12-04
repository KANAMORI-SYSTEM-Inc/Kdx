namespace Kdx.Contracts.DTOs
{
    public class ControlBox
    {
        public int PlcId { get; set; }

        public int BoxNumber { get; set; }

        public string ManualMode { get; set; } = string.Empty;

        public string ManualButton { get; set; } = string.Empty;

        public string? ErrorReset { get; set; }
    }
}
