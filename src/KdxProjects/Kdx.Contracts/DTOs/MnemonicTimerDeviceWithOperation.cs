namespace Kdx.Contracts.DTOs
{
    public class MnemonicTimerDeviceWithOperation
    {
        public MnemonicTimerDevice Timer { get; set; } = default!;
        public Operation Operation { get; set; } = default!;

    }
}
