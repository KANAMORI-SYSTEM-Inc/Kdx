using System.ComponentModel;

namespace Kdx.Contracts.Enums
{
    public enum MnemonicOutcoilCount
    {
        [Description("プロセス")]
        Process = 5,

        [Description("詳細工程")]
        ProcessDetail = 10,

        [Description("動作")]
        Operation = 20,

        [Description("シリンダ")]
        CY = 200
    }
}