using Kdx.Contracts.DTOs;
using Kdx.Contracts.DTOs.MnemonicCommon;

namespace Kdx.Core.Application.Strategies
{
    /// <summary>
    /// ON_2 パターンのInterlock回路生成戦略
    /// </summary>
    public sealed class OnMStrategy : IInterlockMnemonicStrategy
    {
        public string Key => "ON_OR";

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
                    rows.Add(LadderRow.AddLDI(interlockIO.IOAddress));
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
