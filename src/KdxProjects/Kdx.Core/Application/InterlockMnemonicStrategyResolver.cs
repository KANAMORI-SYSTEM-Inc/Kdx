namespace Kdx.Core.Application
{
    /// <summary>
    /// InterlockMnemonicStrategy を解決する実装クラス
    /// </summary>
    public sealed class InterlockMnemonicStrategyResolver : IInterlockMnemonicStrategyResolver
    {
        private readonly IReadOnlyDictionary<string, IInterlockMnemonicStrategy> _strategyMap;

        public InterlockMnemonicStrategyResolver(IEnumerable<IInterlockMnemonicStrategy> strategies)
        {
            if (strategies == null)
                throw new ArgumentNullException(nameof(strategies));

            _strategyMap = strategies.ToDictionary(
                s => s.Key,
                s => s,
                StringComparer.OrdinalIgnoreCase
            );

            if (_strategyMap.Count == 0)
                throw new InvalidOperationException("At least one strategy must be registered.");
        }

        public IInterlockMnemonicStrategy Resolve(string key)
        {
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentException("Strategy key cannot be null or empty.", nameof(key));

            if (_strategyMap.TryGetValue(key, out var strategy))
                return strategy;

            throw new KeyNotFoundException($"Interlock mnemonic strategy not found for key: {key}. Available keys: {string.Join(", ", _strategyMap.Keys)}");
        }
    }
}