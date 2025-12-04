namespace Kdx.Contracts.DTOs
{
    public class Cycle
    {
        public int Id { get; set; }

        public int PlcId { get; set; }

        public string? CycleName { get; set; }

        public string StartDevice { get; set; } = "L1000";

        public string ResetDevice { get; set; } = "L1001";

        public string PauseDevice { get; set; } = "L1002";


    }
}
