namespace Kdx.Contracts.DTOs
{
    public class MnemonicTimerDeviceWithDetail
    {
        public MnemonicTimerDevice Timer { get; set; } = default!;
        public ProcessDetail Detail { get; set; } = default!;

    }
}
