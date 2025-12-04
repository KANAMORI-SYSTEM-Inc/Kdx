using Kdx.Contracts.DTOs;
using Kdx.Contracts.DTOs.MnemonicCommon;
using System.Threading;

namespace Kdx.Core.Application.Strategies
{
    /// <summary>
    /// ON_2 パターンのInterlock回路生成戦略
    /// </summary>
    public sealed class AnyStrategy : IInterlockMnemonicStrategy
    {
        public string Key => "ANY";

        public List<LadderCsvRow> Build(InterlockMnemonicContext ctx)
        {
            var rows = new List<LadderCsvRow>();

            InterlockCondition? condition = ctx.Conditions
                .Where(c => c.InterlockId == ctx.Interlock.CylinderId)
                .Where(c => c.InterlockSortId == ctx.Interlock.SortId)
                .FirstOrDefault();

            if (condition == null)
            {
                return rows;

            }
            else
            {
                if (condition.IsOnCondition != null && condition.Device != null)
                {
                    if (condition.IsOnCondition.Value)
                    {
                        rows.Add(LadderRow.AddLD(condition.Device));

                    }
                    else
                    {
                        rows.Add(LadderRow.AddLDI(condition.Device));

                    }
                }

                return rows;
            }
        }
    }
}
