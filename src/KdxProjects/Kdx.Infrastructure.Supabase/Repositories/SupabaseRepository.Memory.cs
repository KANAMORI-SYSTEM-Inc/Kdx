using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Entities;

namespace Kdx.Infrastructure.Supabase.Repositories
{
    /// <summary>
    /// SupabaseRepositoryのMemory関連メソッド
    /// メモリ、メモリプロファイル、ProsTimeの操作を提供
    /// </summary>
    public partial class SupabaseRepository
    {
        #region MemoryProfile Methods

        /// <summary>
        /// メモリプロファイル一覧を取得
        /// </summary>
        /// <returns>メモリプロファイルリスト</returns>
        public async Task<List<MemoryProfile>> GetMemoryProfilesAsync()
        {
            var response = await _supabaseClient
                .From<MemoryProfileEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        /// <summary>
        /// 指定サイクルのメモリプロファイルを取得
        /// </summary>
        /// <param name="cycleId">サイクルID</param>
        /// <returns>メモリプロファイル（存在しない場合はnull）</returns>
        public async Task<MemoryProfile?> GetMemoryProfileByCycleIdAsync(int cycleId)
        {
            var response = await _supabaseClient
                .From<MemoryProfileEntity>()
                .Where(m => m.CycleId == cycleId)
                .Single();
            return response?.ToDto();
        }

        /// <summary>
        /// メモリプロファイルを追加
        /// </summary>
        /// <param name="profile">メモリプロファイル</param>
        public async Task AddMemoryProfileAsync(MemoryProfile profile)
        {
            var entity = MemoryProfileEntity.FromDtoForInsert(profile);
            await _supabaseClient
                .From<MemoryProfileEntityForInsert>()
                .Insert(entity);
        }

        /// <summary>
        /// メモリプロファイルを更新
        /// </summary>
        /// <param name="profile">更新対象のメモリプロファイル</param>
        public async Task UpdateMemoryProfileAsync(MemoryProfile profile)
        {
            var entity = MemoryProfileEntity.FromDto(profile);
            entity.UpdatedAt = DateTime.UtcNow;
            await _supabaseClient
                .From<MemoryProfileEntity>()
                .Where(m => m.CycleId == profile.CycleId)
                .Update(entity);
        }

        /// <summary>
        /// 指定サイクルのメモリプロファイルを削除
        /// </summary>
        /// <param name="cycleId">サイクルID</param>
        public async Task DeleteMemoryProfileAsync(int cycleId)
        {
            await _supabaseClient
                .From<MemoryProfileEntity>()
                .Where(m => m.CycleId == cycleId)
                .Delete();
        }

        #endregion
    }
}
