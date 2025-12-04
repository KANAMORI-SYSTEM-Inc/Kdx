namespace Kdx.Contracts.DTOs
{
    public class Definitions
    {
        public int ID { get; set; }

        public string DefName { get; set; } = string.Empty;

        public int OutCoilNumber { get; set; }

        public string? Description { get; set; }

        public string? Comment1 { get; set; }

        public string? Comment2 { get; set; }

        public string Label { get; set; } = "M";

        public string Category { get; set; } = "Nan";

        public int MnemonicId { get; set; }

        public int MemoryCategoryId { get; set; }

    }
}
