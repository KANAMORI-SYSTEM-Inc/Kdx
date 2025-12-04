using Kdx.Contracts.DTOs;
using Kdx.Contracts.Interfaces;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Diagnostics;

namespace Kdx.Infrastructure.Services
{
    /// <summary>
    /// メモリデータの操作を行うサービス実装
    /// Infrastructure層に配置することで、ビジネスロジックをUI層から分離
    /// </summary>
    public class MemoryService : IMemoryService
    {
        private readonly ISupabaseRepository _repository;

        public MemoryService(ISupabaseRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        public List<Memory> GetMemories(int plcId)
        {
            return Task.Run(async () => await _repository.GetMemoriesAsync(plcId)).GetAwaiter().GetResult();
        }

        public List<MemoryCategory> GetMemoryCategories()
        {
            return Task.Run(async () => await _repository.GetMemoryCategoriesAsync()).GetAwaiter().GetResult();
        }

        private (int PlcId, string Device) GetMemoryKey(Memory memory)
        {
            return string.IsNullOrEmpty(memory.Device)
                ? throw new ArgumentException("Memory Device cannot be null or empty for key generation.", nameof(memory.Device))
                : (memory.PlcId, memory.Device);
        }

        public void SaveMemories(int plcId, List<Memory> memories, Action<string>? progressCallback = null)
        {
            if (memories == null || !memories.Any())
            {
                progressCallback?.Invoke($"保存対象のメモリデータがありません (PlcId: {plcId})。");
                return;
            }

            try
            {
                // 1. 渡された plcId を使用して、関連する既存レコードを取得
                var existingForThisPlcId = GetMemories(plcId);

                // 2. 取得した既存レコードからルックアップ用辞書を作成
                var existingLookup = new Dictionary<(int PlcId, string Device), Memory>();
                foreach (var mem in existingForThisPlcId)
                {
                    if (mem.PlcId == plcId && !string.IsNullOrEmpty(mem.Device))
                    {
                        existingLookup[GetMemoryKey(mem)] = mem;
                    }
                }

                // 3. 保存対象のメモリデータをフィルタリング
                var memoriesToSave = new List<Memory>();
                int skippedCount = 0;
                
                for (int i = 0; i < memories.Count; i++)
                {
                    var memoryToSave = memories[i];

                    if (memoryToSave == null)
                    {
                        progressCallback?.Invoke($"[{i + 1}/{memories.Count}] スキップ: null のメモリデータです。");
                        skippedCount++;
                        continue;
                    }

                    if (memoryToSave.PlcId != plcId)
                    {
                        progressCallback?.Invoke($"[{i + 1}/{memories.Count}] スキップ: PlcId ({memoryToSave.PlcId.ToString() ?? "null"}) が指定された PlcId ({plcId}) と一致しません。Device: {memoryToSave.Device}");
                        skippedCount++;
                        continue;
                    }

                    if (string.IsNullOrEmpty(memoryToSave.Device))
                    {
                        progressCallback?.Invoke($"[{i + 1}/{memories.Count}] スキップ: Device が null または空です (PlcId: {plcId})。");
                        skippedCount++;
                        continue;
                    }

                    // 有効なメモリデータを保存リストに追加
                    memoriesToSave.Add(memoryToSave);
                }

                // 4. 一括でSupabaseに保存または更新
                if (memoriesToSave.Any())
                {
                    progressCallback?.Invoke($"一括保存中: {memoriesToSave.Count} 件のメモリデータ (PlcId: {plcId})");
                    
                    // バッチ処理で一括保存
                    Task.Run(async () => await _repository.SaveOrUpdateMemoriesBatchAsync(memoriesToSave)).GetAwaiter().GetResult();
                    
                    progressCallback?.Invoke($"メモリデータの保存が完了しました (PlcId: {plcId}, 保存件数: {memoriesToSave.Count}, スキップ: {skippedCount} 件)。");
                }
                else
                {
                    progressCallback?.Invoke($"保存可能なメモリデータがありませんでした (PlcId: {plcId}, スキップ: {skippedCount} 件)。");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ERROR] SaveMemories 処理中にエラーが発生しました (PlcId={plcId}): {ex.Message}");
                progressCallback?.Invoke($"エラーが発生しました (PlcId={plcId}): {ex.Message}");
                throw;
            }
        }
    }
}