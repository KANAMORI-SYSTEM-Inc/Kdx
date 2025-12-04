namespace Kdx.Contracts.DTOs
{
    public class ProcessDetail
    {
        public int Id { get; set; }
        public int ProcessId { get; set; }
        public int? OperationId { get; set; }
        public string? DetailName { get; set; }
        public string? StartSensor { get; set; }
        public int? CategoryId { get; set; }
        public string? FinishSensor { get; set; }
        public int? BlockNumber { get; set; }
        public string? SkipMode { get; set; }
        public int? CycleId { get; set; }
        public int? SortNumber { get; set; }
        public string? Comment { get; set; }
        public string? ILStart { get; set; }
        public int? StartTimerId { get; set; }
        public bool? IsResetAfter { get; set; }
    }
}