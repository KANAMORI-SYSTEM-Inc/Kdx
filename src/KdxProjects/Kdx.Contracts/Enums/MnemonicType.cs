using System.ComponentModel;

namespace Kdx.Contracts.Enums
{
    public enum MnemonicType
    {
        [Description("プロセス")]
        Process = 1,

        [Description("詳細工程")]
        ProcessDetail = 2,

        [Description("動作")]
        Operation = 3,

        [Description("シリンダ")]
        CY = 4
    }
}