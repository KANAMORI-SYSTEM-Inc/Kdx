namespace Kdx.Contracts.DTOs
{
    public class InterlockPrecondition2
    {
        public int Id { get; set; }

        public bool IsEnableProcess { get; set; } = false;

        public string? InterlockMode { get; set; }

        public int? StartDetailId { get; set; }

        public int? EndDetailId { get; set; }

    }
}
