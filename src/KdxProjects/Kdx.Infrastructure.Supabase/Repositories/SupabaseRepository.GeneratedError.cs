using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Entities;
using static Supabase.Postgrest.Constants;

namespace Kdx.Infrastructure.Supabase.Repositories
{
    public partial class SupabaseRepository
    {
        #region GeneratedError

        /// <summary>
        /// 指定されたPLCの生成エラーを取得します。
        /// </summary>
        public async Task<List<GeneratedError>> GetGeneratedErrorsByPlcIdAsync(int plcId)
        {
            try
            {
                var response = await _supabaseClient
                    .From<GeneratedErrorEntity>()
                    .Filter("plc_id", Operator.Equals, plcId.ToString())
                    .Order("error_num", Ordering.Ascending)
                    .Get();

                return response.Models.Select(e => e.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetGeneratedErrorsByPlcIdAsync error: {ex.Message}");
                return new List<GeneratedError>();
            }
        }

        /// <summary>
        /// 指定されたPLCとMnemonicIdの生成エラーを取得します。
        /// </summary>
        public async Task<List<GeneratedError>> GetGeneratedErrorsByPlcIdAndMnemonicIdAsync(int plcId, int mnemonicId)
        {
            try
            {
                var response = await _supabaseClient
                    .From<GeneratedErrorEntity>()
                    .Filter("plc_id", Operator.Equals, plcId.ToString())
                    .Filter("mnemonic_id", Operator.Equals, mnemonicId.ToString())
                    .Order("error_num", Ordering.Ascending)
                    .Get();

                return response.Models.Select(e => e.ToDto()).ToList();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetGeneratedErrorsByPlcIdAndMnemonicIdAsync error: {ex.Message}");
                return new List<GeneratedError>();
            }
        }

        /// <summary>
        /// 生成エラーをバッチで保存します（Upsert）。
        /// </summary>
        public async Task SaveGeneratedErrorsBatchAsync(List<GeneratedError> errors)
        {
            if (errors == null || errors.Count == 0) return;

            try
            {
                var entities = errors.Select(GeneratedErrorEntity.FromDto).ToList();

                // バッチサイズを100件ずつに分割して処理
                const int batchSize = 100;
                for (int i = 0; i < entities.Count; i += batchSize)
                {
                    var batch = entities.Skip(i).Take(batchSize).ToList();
                    await _supabaseClient
                        .From<GeneratedErrorEntity>()
                        .Upsert(batch);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"SaveGeneratedErrorsBatchAsync error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 指定されたPLCの生成エラーを全て削除します。
        /// </summary>
        public async Task DeleteGeneratedErrorsByPlcIdAsync(int plcId)
        {
            try
            {
                await _supabaseClient
                    .From<GeneratedErrorEntity>()
                    .Filter("plc_id", Operator.Equals, plcId.ToString())
                    .Delete();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteGeneratedErrorsByPlcIdAsync error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 指定されたPLCとMnemonicIdの生成エラーを削除します。
        /// </summary>
        public async Task DeleteGeneratedErrorsByPlcIdAndMnemonicIdAsync(int plcId, int mnemonicId)
        {
            try
            {
                await _supabaseClient
                    .From<GeneratedErrorEntity>()
                    .Filter("plc_id", Operator.Equals, plcId.ToString())
                    .Filter("mnemonic_id", Operator.Equals, mnemonicId.ToString())
                    .Delete();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"DeleteGeneratedErrorsByPlcIdAndMnemonicIdAsync error: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 指定されたPLCの次のエラー番号を取得します。
        /// </summary>
        public async Task<int> GetNextErrorNumForPlcAsync(int plcId)
        {
            try
            {
                var response = await _supabaseClient
                    .From<GeneratedErrorEntity>()
                    .Filter("plc_id", Operator.Equals, plcId.ToString())
                    .Order("error_num", Ordering.Descending)
                    .Limit(1)
                    .Get();

                if (response.Models.Count > 0)
                {
                    return response.Models[0].ErrorNum + 1;
                }
                return 1; // 最初のエラー番号
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"GetNextErrorNumForPlcAsync error: {ex.Message}");
                return 1;
            }
        }

        #endregion
    }
}
