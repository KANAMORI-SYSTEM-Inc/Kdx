using Kdx.Contracts.DTOs;

namespace Kdx.Contracts.Interfaces
{
    /// <summary>
    /// エラー集約サービスのインターフェース
    /// </summary>
    public interface IErrorAggregator
    {
        /// <summary>
        /// エラーを追加
        /// </summary>
        void AddError(OutputError error);

        /// <summary>
        /// 複数のエラーを追加
        /// </summary>
        void AddErrors(IEnumerable<OutputError> errors);

        /// <summary>
        /// すべてのエラーを取得
        /// </summary>
        IReadOnlyList<OutputError> GetAllErrors();

        /// <summary>
        /// エラーをクリア
        /// </summary>
        void Clear();
    }
}