using Kdx.Contracts.DTOs;
using Kdx.Contracts.Interfaces;
using System.Diagnostics;

namespace Kdx.Core.Application
{
    /// <summary>
    /// エラー集約サービスの実装
    /// </summary>
    public class ErrorAggregator : IErrorAggregator
    {
        private readonly List<OutputError> _errors = new();
        private readonly object _lock = new();
        private readonly int? _mnemonicId;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="mnemonicId">デフォルトのニモニックID（オプション）</param>
        public ErrorAggregator(int? mnemonicId = null)
        {
            _mnemonicId = mnemonicId;
        }

        /// <summary>
        /// エラーを追加
        /// </summary>
        public void AddError(OutputError error)
        {
            Debug.WriteLine($"{error.Message} RecordId:{error.RecordId} MnemonicId:{error.MnemonicId}");

            if (_mnemonicId != null)
                error.MnemonicId = _mnemonicId;

            lock (_lock)
            {
                _errors.Add(error);
            }
        }

        /// <summary>
        /// 複数のエラーを追加
        /// </summary>
        public void AddErrors(IEnumerable<OutputError> errors)
        {
            if (errors == null) return;

            lock (_lock)
            {
                _errors.AddRange(errors);
            }
        }

        /// <summary>
        /// すべてのエラーを取得
        /// </summary>
        public IReadOnlyList<OutputError> GetAllErrors()
        {
            lock (_lock)
            {
                return _errors.ToList().AsReadOnly();
            }
        }

        /// <summary>
        /// エラーをクリア
        /// </summary>
        public void Clear()
        {
            lock (_lock)
            {
                _errors.Clear();
            }
        }
    }
}