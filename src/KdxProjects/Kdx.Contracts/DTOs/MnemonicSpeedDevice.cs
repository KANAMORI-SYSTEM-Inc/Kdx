namespace Kdx.Contracts.DTOs
{
    public class MnemonicSpeedDevice
    {
        public int ID { get; set; }

        public int CylinderId { get; set; }

        public string Device { get; set; } = "D0";

        public int PlcId { get; set; }

        public int? CycleId { get; set; }
    }
}