using Kdx.Contracts.DTOs;
using Kdx.Contracts.DTOs.MnemonicCommon;

namespace Kdx.Core.Application.Strategies
{
    /// <summary>
    /// ON_2 パターンのInterlock回路生成戦略
    /// </summary>
    public sealed class Off1Strategy : IInterlockMnemonicStrategy
    {
        public string Key => "OFF_1";

        public List<LadderCsvRow> Build(InterlockMnemonicContext ctx)
        {
            var rows = new List<LadderCsvRow>();

            bool isFirst = true;

            foreach (var interlockIO in ctx.InterlockIOs)
            {
                if (interlockIO.IOAddress == null)
                {
                    throw new InvalidOperationException($"ON_1 IOAddress is null Interlock (CylinderId:{ctx.Interlock.CylinderId}, SortId:{ctx.Interlock.SortId})");
                }

                if (isFirst)
                {
                    rows.Add(LadderRow.AddLD(interlockIO.IOAddress));
                }
                else
                {
                    rows.Add(LadderRow.AddOR(interlockIO.IOAddress));
                }
            }
            return rows;
        }
    }
}
