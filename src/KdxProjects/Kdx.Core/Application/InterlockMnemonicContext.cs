using Kdx.Contracts.DTOs;

namespace Kdx.Core.Application
{
    /// <summary>
    /// Interlockニモニック生成に必要な全パラメータをまとめたコンテキスト
    /// </summary>
    public sealed record InterlockMnemonicContext(
        int ErrorNumber,
        string ErrorDevice,
        string ErrorOutputDevice,
        int PlcId,
        Cycle Cycle,
        IReadOnlyList<MnemonicDeviceWithProcessDetail> MnemonicDevices,
        Cylinder Cylinder,
        Interlock Interlock,
        InterlockPrecondition1? Precondition1,
        InterlockPrecondition2? Precondition2,
        IReadOnlyList<InterlockCondition> Conditions,
        IReadOnlyList<InterlockIO> InterlockIOs
    );
}
