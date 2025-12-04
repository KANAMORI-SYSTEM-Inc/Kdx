namespace Kdx.Contracts.DTOs
{
    public class Memory
    {
        public int PlcId { get; set; }

        public string Device { get; set; } = string.Empty;

        public int? MemoryCategory { get; set; }

        public int? DeviceNumber { get; set; }

        public string? DeviceNumber1 { get; set; }

        public string? DeviceNumber2 { get; set; }

        public string? Category { get; set; }

        public string? Row_1 { get; set; }

        public string? Row_2 { get; set; }

        public string? Row_3 { get; set; }

        public string? Row_4 { get; set; }

        public string? Direct_Input { get; set; }

        public string? Confirm { get; set; }

        public string? Note { get; set; }

        public string? GOT { get; set; }

        public int? MnemonicId { get; set; }

        public int? RecordId { get; set; }

        public int? OutcoilNumber { get; set; }

    }
}