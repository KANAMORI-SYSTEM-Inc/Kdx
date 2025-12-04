using Kdx.Contracts.DTOs;

namespace Kdx.Core.Application
{
    /// <summary>
    /// Interlockニモニック生成の戦略インターフェース
    /// </summary>
    public interface IInterlockMnemonicStrategy
    {
        /// <summary>
        /// このStrategyが担当するキー（例: "ON_1", "ON_2"...）
        /// </summary>
        string Key { get; }

        /// <summary>
        /// コンテキストからLadder CSVの行を生成
        /// </summary>
        /// <param name="ctx">生成に必要な全パラメータ</param>
        /// <returns>生成されたLadder CSV行のリスト</returns>
        List<LadderCsvRow> Build(InterlockMnemonicContext ctx);
    }
}
