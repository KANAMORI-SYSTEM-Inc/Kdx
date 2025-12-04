namespace Kdx.Core.Application
{
    /// <summary>
    /// InterlockMnemonicStrategy を解決するインターフェース
    /// </summary>
    public interface IInterlockMnemonicStrategyResolver
    {
        /// <summary>
        /// キーに対応する戦略を解決
        /// </summary>
        /// <param name="key">戦略キー（例: "ON_1", "ON_2"）</param>
        /// <returns>対応する戦略実装</returns>
        /// <exception cref="KeyNotFoundException">キーに対応する戦略が見つからない場合</exception>
        IInterlockMnemonicStrategy Resolve(string key);
    }
}