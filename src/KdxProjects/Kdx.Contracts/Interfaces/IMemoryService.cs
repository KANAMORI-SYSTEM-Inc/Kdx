using Kdx.Contracts.DTOs;

namespace Kdx.Contracts.Interfaces
{
    /// <summary>
    /// メモリデータの操作を行うサービスインターフェース
    /// </summary>
    public interface IMemoryService
    {
        /// <summary>
        /// 指定されたPLCのメモリ情報を取得
        /// </summary>
        List<Memory> GetMemories(int plcId);

        /// <summary>
        /// メモリカテゴリの一覧を取得
        /// </summary>
        List<MemoryCategory> GetMemoryCategories();

        /// <summary>
        /// メモリ情報を保存
        /// </summary>
        void SaveMemories(int plcId, List<Memory> memories, Action<string>? progressCallback = null);
    }
}