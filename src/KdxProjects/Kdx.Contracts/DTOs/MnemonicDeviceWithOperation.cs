namespace Kdx.Contracts.DTOs
{
    public class MnemonicDeviceWithOperation
    {
        public Kdx.Contracts.DTOs.MnemonicDevice Mnemonic { get; set; } = default!;
        public Operation Operation { get; set; } = default!;

    }
}
