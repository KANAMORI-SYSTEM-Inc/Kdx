using Kdx.Contracts.DTOs;
using Kdx.Contracts.DTOs.MnemonicCommon;

namespace Kdx.Core.Application.Strategies
{
    /// <summary>
    /// ON_2 パターンのInterlock回路生成戦略
    /// </summary>
    public sealed class OnOrStrategy : IInterlockMnemonicStrategy
    {
        public string Key => "ON_OR";

        public List<LadderCsvRow> Build(InterlockMnemonicContext ctx)
        {
            var rows = new List<LadderCsvRow>();

            foreach (var interlockIO in ctx.InterlockIOs)
            {
                if (interlockIO.IOAddress == null)
                {
                    throw new InvalidOperationException($"ON_OR IOAddress is null Interlock (CylinderId:{ctx.Interlock.CylinderId}, SortId:{ctx.Interlock.SortId})");
                }
                if (rows.Count == 0)
                {
                    rows.Add(LadderRow.AddLDI(interlockIO.IOAddress));
                }
                else
                {
                    rows.Add(LadderRow.AddANI(interlockIO.IOAddress));
                }
            }

            foreach (var interlockIO in ctx.InterlockIOs)
            {
                if (interlockIO.IOAddress == null)
                {
                    throw new InvalidOperationException($"ON_OR IOAddress is null Interlock (CylinderId:{ctx.Interlock.CylinderId}, SortId:{ctx.Interlock.SortId})");
                }
                if (rows.Count == 0)
                {
                    rows.Add(LadderRow.AddLD(interlockIO.IOAddress));
                }
                else
                {
                    rows.Add(LadderRow.AddAND(interlockIO.IOAddress));
                }
            }
            rows.Add(LadderRow.AddORB());

            return rows;
        }
    }
}
