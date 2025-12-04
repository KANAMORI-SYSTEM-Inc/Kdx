namespace Kdx.Contracts.DTOs
{
    public class Machine
    {
        public int MachineNameId { get; set; }

        public int DriveSubId { get; set; }

        public bool UseValveRetention { get; set; } = false;

        public string? Description { get; set; }
    }
}
