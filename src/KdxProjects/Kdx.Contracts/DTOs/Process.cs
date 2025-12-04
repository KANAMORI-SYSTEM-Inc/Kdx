namespace Kdx.Contracts.DTOs
{
    public class Process
    {
        public int Id { get; set; }

        public string? ProcessName { get; set; }

        public int? CycleId { get; set; }

        public string? TestStart { get; set; }

        public string? TestCondition { get; set; }

        public string? TestMode { get; set; }

        public string? AutoMode { get; set; }

        public string? AutoStart { get; set; }

        public int? ProcessCategoryId { get; set; }

        public string? ILStart { get; set; }

        public string? Comment1 { get; set; }

        public string? Comment2 { get; set; }

        public int? SortNumber { get; set; }

    }
}