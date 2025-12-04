namespace Kdx.Contracts.DTOs
{
        public class ProcessError 
    {        public int? PlcId { get; set; }        public int? CycleId { get; set; }        public string? Device { get; set; }        public int? MnemonicId { get; set; }        public int? RecordId { get; set; }        public int? AlarmId { get; set; }        public int? ErrorNum { get; set; }        public string? Comment1 { get; set; }        public string? Comment2 { get; set; }        public string? Comment3 { get; set; }        public string? Comment4 { get; set; }        public string? AlarmComment { get; set; }        public string? MessageComment { get; set; }        public int? ErrorTime { get; set; }        public string? ErrorTimeDevice { get; set; }
        
        // Note: ErrorCountTime doesn't exist in the database table
        // This was likely a duplicate of ErrorTime in the original Access database

    }
}
