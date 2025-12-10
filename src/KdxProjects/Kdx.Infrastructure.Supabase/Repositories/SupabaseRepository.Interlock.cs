using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Entities;
using System.Diagnostics;
using static global::Supabase.Postgrest.Constants;

namespace Kdx.Infrastructure.Supabase.Repositories
{
    /// <summary>
    /// SupabaseRepositoryのInterlock関連メソッド
    /// インターロック、インターロック条件、インターロックIOの操作を提供
    /// </summary>
    public partial class SupabaseRepository
    {
        #region Interlock Methods

        /// <summary>
        /// 指定PLCのインターロック一覧を取得
        /// </summary>
        /// <param name="plcId">PLC ID</param>
        /// <returns>インターロックリスト</returns>
        public async Task<List<Interlock>> GetInterlocksByPlcIdAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<InterlockEntity>()
                .Where(i => i.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        /// <summary>
        /// 指定シリンダーのインターロック一覧を取得
        /// </summary>
        /// <param name="cylinderId">シリンダーID</param>
        /// <returns>インターロックリスト</returns>
        public async Task<List<Interlock>> GetInterlocksByCylindrIdAsync(int cylinderId)
        {
            var response = await _supabaseClient
                .From<InterlockEntity>()
                .Where(i => i.CylinderId == cylinderId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        /// <summary>
        /// インターロックを追加または更新
        /// </summary>
        /// <param name="interlock">インターロック</param>
        public async Task UpsertInterlockAsync(Interlock interlock)
        {
            var entity = InterlockEntity.FromDto(interlock);
            await _supabaseClient
                .From<InterlockEntity>()
                .Upsert(entity);
        }

        /// <summary>
        /// 複数のインターロックを一括で追加または更新
        /// 複合キー(PlcId, CylinderId, SortId)で重複を判定
        /// </summary>
        /// <param name="interlocks">インターロックリスト</param>
        public async Task UpsertInterlocksAsync(List<Interlock> interlocks)
        {
            // 重複を除去（同じPlcId + CylinderId + SortIdを持つレコードは1つだけ保持）
            var uniqueInterlocks = interlocks
                .GroupBy(i => new { i.PlcId, i.CylinderId, i.SortId })
                .Select(g => g.First())
                .ToList();

            // 既存レコードを取得して、Insert/Updateを判断
            var plcIds = uniqueInterlocks.Select(i => i.PlcId).Distinct().ToList();
            if (!plcIds.Any()) return;

            var existingRecords = new List<Interlock>();
            foreach (var plcId in plcIds)
            {
                var records = await _supabaseClient
                    .From<InterlockEntity>()
                    .Where(i => i.PlcId == plcId)
                    .Get();
                existingRecords.AddRange(records.Models.Select(e => e.ToDto()));
            }

            // 新規作成と更新に分ける
            var toInsert = new List<Interlock>();
            var toUpdate = new List<Interlock>();

            foreach (var interlock in uniqueInterlocks)
            {
                var existing = existingRecords.FirstOrDefault(e =>
                    e.PlcId == interlock.PlcId &&
                    e.CylinderId == interlock.CylinderId &&
                    e.SortId == interlock.SortId);

                if (existing != null)
                {
                    toUpdate.Add(interlock);
                }
                else
                {
                    toInsert.Add(interlock);
                }
            }

            // 新規作成: Insert
            if (toInsert.Any())
            {
                var newEntities = toInsert.Select(i => InterlockEntity.FromDto(i)).ToList();
                await _supabaseClient
                    .From<InterlockEntity>()
                    .Insert(newEntities);
            }

            // 既存更新: Update (1件ずつ更新)
            if (toUpdate.Any())
            {
                foreach (var interlock in toUpdate)
                {
                    var entity = InterlockEntity.FromDto(interlock);
                    await _supabaseClient
                        .From<InterlockEntity>()
                        .Where(i => i.CylinderId == entity.CylinderId)
                        .Where(i => i.SortId == entity.SortId)
                        .Update(entity);
                }
            }
        }

        /// <summary>
        /// インターロックを削除
        /// </summary>
        /// <param name="interlock">削除対象のインターロック</param>
        public async Task DeleteInterlockAsync(Interlock interlock)
        {
            Debug.WriteLine($"[DeleteInterlockAsync] 削除開始");
            Debug.WriteLine($"  CylinderId: {interlock.CylinderId}");
            Debug.WriteLine($"  SortId: {interlock.SortId}");

            try
            {
                // 削除前に対象レコードが存在するか確認
                var existingRecords = await _supabaseClient
                    .From<InterlockEntity>()
                    .Filter("CylinderId", Operator.Equals, interlock.CylinderId.ToString())
                    .Filter("SortId", Operator.Equals, interlock.SortId.ToString())
                    .Get();

                Debug.WriteLine($"  削除前のレコード数: {existingRecords?.Models?.Count ?? 0}");

                // 削除実行 - Filterを使用
                await _supabaseClient
                    .From<InterlockEntity>()
                    .Filter("CylinderId", Operator.Equals, interlock.CylinderId.ToString())
                    .Filter("SortId", Operator.Equals, interlock.SortId.ToString())
                    .Delete();

                Debug.WriteLine($"  削除完了");

                // 削除後に確認
                var afterRecords = await _supabaseClient
                    .From<InterlockEntity>()
                    .Filter("CylinderId", Operator.Equals, interlock.CylinderId.ToString())
                    .Filter("SortId", Operator.Equals, interlock.SortId.ToString())
                    .Get();

                Debug.WriteLine($"  削除後のレコード数: {afterRecords?.Models?.Count ?? 0}");

                if (afterRecords?.Models?.Any() == true)
                {
                    Debug.WriteLine($"  警告: 削除後もレコードが残っています！");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DeleteInterlockAsync] エラー: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// 複数のインターロックを一括削除
        /// </summary>
        /// <param name="interlocks">削除対象のインターロックリスト</param>
        public async Task DeleteInterlocksAsync(List<Interlock> interlocks)
        {
            foreach (var interlock in interlocks)
            {
                await _supabaseClient
                    .From<InterlockEntity>()
                    .Filter("CylinderId", Operator.Equals, interlock.CylinderId.ToString())
                    .Filter("SortId", Operator.Equals, interlock.SortId.ToString())
                    .Delete();
            }
        }

        /// <summary>
        /// インターロックを更新
        /// </summary>
        /// <param name="interlock">更新対象のインターロック</param>
        public async Task UpdateInterlockAsync(Interlock interlock)
        {
            var entity = InterlockEntity.FromDto(interlock);
            await _supabaseClient
                .From<InterlockEntity>()
                .Where(e => e.CylinderId == interlock.CylinderId)
                .Where(e => e.SortId == interlock.SortId)
                .Update(entity);
        }

        #endregion

        #region InterlockCondition Methods

        /// <summary>
        /// 指定シリンダーのインターロック条件一覧を取得
        /// </summary>
        /// <param name="cylinderId">シリンダーID</param>
        /// <returns>インターロック条件リスト</returns>
        public async Task<List<InterlockConditionDTO>> GetInterlockConditionsByCylinderIdAsync(int cylinderId)
        {
            var response = await _supabaseClient
                .From<InterlockConditionDTOEntity>()
                .Where(ic => ic.CylinderId == cylinderId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        /// <summary>
        /// インターロック条件を追加または更新
        /// ナビゲーションプロパティは除外してクリーンコピーを作成
        /// </summary>
        /// <param name="interlockCondition">インターロック条件</param>
        public async Task UpsertInterlockConditionAsync(InterlockConditionDTO interlockCondition)
        {
            // ナビゲーションプロパティを除外したクリーンコピーを作成
            var cleanCondition = new InterlockCondition
            {
                CylinderId = interlockCondition.CylinderId,
                ConditionNumber = interlockCondition.ConditionNumber,
                InterlockSortId = interlockCondition.InterlockSortId,
                ConditionTypeId = interlockCondition.ConditionTypeId,
                Name = interlockCondition.Name,
                Device = interlockCondition.Device,
                IsOnCondition = interlockCondition.IsOnCondition,
                Comment1 = interlockCondition.Comment1,
                Comment2 = interlockCondition.Comment2
            };

            var entity = InterlockConditionEntity.FromDto(cleanCondition);
            await _supabaseClient
                .From<InterlockConditionEntity>()
                .Upsert(entity);
        }

        /// <summary>
        /// 複数のインターロック条件を一括で追加または更新
        /// 複合キー(CylinderId, ConditionNumber, InterlockSortId)で重複を判定
        /// </summary>
        /// <param name="interlockConditions">インターロック条件リスト</param>
        public async Task UpsertInterlockConditionsAsync(List<InterlockConditionDTO> interlockConditions)
        {
            // ナビゲーションプロパティを除外したクリーンコピーを作成
            var cleanConditions = interlockConditions.Select(c => new InterlockCondition
            {
                CylinderId = c.CylinderId,
                ConditionNumber = c.ConditionNumber,
                InterlockSortId = c.InterlockSortId,
                ConditionTypeId = c.ConditionTypeId,
                Name = c.Name,
                Device = c.Device,
                IsOnCondition = c.IsOnCondition,
                Comment1 = c.Comment1,
                Comment2 = c.Comment2
            }).ToList();

            // 重複を除去（同じCylinderId + ConditionNumber + InterlockSortIdを持つレコードは1つだけ保持）
            var uniqueConditions = cleanConditions
                .GroupBy(c => new { c.CylinderId, c.ConditionNumber, c.InterlockSortId })
                .Select(g => g.First())
                .ToList();

            // 既存レコードを取得して、Insert/Updateを判断
            var cylinderIds = uniqueConditions.Select(c => c.CylinderId).Distinct().ToList();
            if (!cylinderIds.Any()) return;

            var existingRecords = new List<InterlockCondition>();
            foreach (var cylinderId in cylinderIds)
            {
                var records = await _supabaseClient
                    .From<InterlockConditionEntity>()
                    .Where(c => c.CylinderId == cylinderId)
                    .Get();
                existingRecords.AddRange(records.Models.Select(e => e.ToDto()));
            }

            // 新規作成と更新に分ける
            var toInsert = new List<InterlockCondition>();
            var toUpdate = new List<InterlockCondition>();

            foreach (var condition in uniqueConditions)
            {
                var existing = existingRecords.FirstOrDefault(e =>
                    e.CylinderId == condition.CylinderId &&
                    e.ConditionNumber == condition.ConditionNumber &&
                    e.InterlockSortId == condition.InterlockSortId);

                if (existing != null)
                {
                    toUpdate.Add(condition);
                }
                else
                {
                    toInsert.Add(condition);
                }
            }

            // 新規作成: Insert
            if (toInsert.Any())
            {
                var newEntities = toInsert.Select(c => InterlockConditionEntity.FromDto(c)).ToList();
                await _supabaseClient
                    .From<InterlockConditionEntity>()
                    .Insert(newEntities);
            }

            // 既存更新: Update (1件ずつ更新)
            if (toUpdate.Any())
            {
                foreach (var condition in toUpdate)
                {
                    var entity = InterlockConditionEntity.FromDto(condition);
                    await _supabaseClient
                        .From<InterlockConditionEntity>()
                        .Where(c => c.CylinderId == entity.CylinderId)
                        .Where(c => c.ConditionNumber == entity.ConditionNumber)
                        .Where(c => c.InterlockSortId == entity.InterlockSortId)
                        .Update(entity);
                }
            }
        }

        /// <summary>
        /// インターロック条件を削除
        /// </summary>
        /// <param name="interlockCondition">削除対象のインターロック条件</param>
        public async Task DeleteInterlockConditionAsync(InterlockConditionDTO interlockCondition)
        {
            Debug.WriteLine($"[DeleteInterlockConditionAsync] 削除開始");
            Debug.WriteLine($"  CylinderId: {interlockCondition.CylinderId}");
            Debug.WriteLine($"  ConditionNumber: {interlockCondition.ConditionNumber}");
            Debug.WriteLine($"  InterlockSortId: {interlockCondition.InterlockSortId}");

            try
            {
                // 削除前に対象レコードが存在するか確認
                var existingRecords = await _supabaseClient
                    .From<InterlockConditionEntity>()
                    .Filter("CylinderId", Operator.Equals, interlockCondition.CylinderId.ToString())
                    .Filter("ConditionNumber", Operator.Equals, interlockCondition.ConditionNumber.ToString())
                    .Filter("InterlockSortId", Operator.Equals, interlockCondition.InterlockSortId.ToString())
                    .Get();

                Debug.WriteLine($"  削除前のレコード数: {existingRecords?.Models?.Count ?? 0}");

                if (existingRecords?.Models?.Any() == true)
                {
                    foreach (var record in existingRecords.Models)
                    {
                        Debug.WriteLine($"    対象レコード: CylinderId={record.CylinderId}, ConditionNumber={record.ConditionNumber}, InterlockSortId={record.InterlockSortId}");
                    }
                }

                // 削除実行 - Filterを使用
                await _supabaseClient
                    .From<InterlockConditionEntity>()
                    .Filter("CylinderId", Operator.Equals, interlockCondition.CylinderId.ToString())
                    .Filter("ConditionNumber", Operator.Equals, interlockCondition.ConditionNumber.ToString())
                    .Filter("InterlockSortId", Operator.Equals, interlockCondition.InterlockSortId.ToString())
                    .Delete();

                Debug.WriteLine($"  削除完了");

                // 削除後に確認
                var afterRecords = await _supabaseClient
                    .From<InterlockConditionEntity>()
                    .Filter("CylinderId", Operator.Equals, interlockCondition.CylinderId.ToString())
                    .Filter("ConditionNumber", Operator.Equals, interlockCondition.ConditionNumber.ToString())
                    .Filter("InterlockSortId", Operator.Equals, interlockCondition.InterlockSortId.ToString())
                    .Get();

                Debug.WriteLine($"  削除後のレコード数: {afterRecords?.Models?.Count ?? 0}");

                if (afterRecords?.Models?.Any() == true)
                {
                    Debug.WriteLine($"  警告: 削除後もレコードが残っています！");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DeleteInterlockConditionAsync] エラー: {ex.Message}");
                Debug.WriteLine($"  スタックトレース: {ex.StackTrace}");
                throw;
            }
        }

        /// <summary>
        /// 複数のインターロック条件を一括削除
        /// </summary>
        /// <param name="interlockConditions">削除対象のインターロック条件リスト</param>
        public async Task DeleteInterlockConditionsAsync(List<InterlockConditionDTO> interlockConditions)
        {
            foreach (var condition in interlockConditions)
            {
                await _supabaseClient
                    .From<InterlockConditionEntity>()
                    .Filter("CylinderId", Operator.Equals, condition.CylinderId.ToString())
                    .Filter("ConditionNumber", Operator.Equals, condition.ConditionNumber.ToString())
                    .Filter("InterlockSortId", Operator.Equals, condition.InterlockSortId.ToString())
                    .Delete();
            }
        }

        /// <summary>
        /// インターロック条件を更新
        /// </summary>
        /// <param name="interlockCondition">更新対象のインターロック条件</param>
        public async Task UpdateInterlockConditionAsync(InterlockConditionDTO interlockCondition)
        {
            var entity = InterlockConditionDTOEntity.FromDto(interlockCondition);
            await _supabaseClient
                .From<InterlockConditionDTOEntity>()
                .Where(e => e.CylinderId == interlockCondition.CylinderId)
                .Where(e => e.InterlockSortId == interlockCondition.InterlockSortId)
                .Where(e => e.ConditionNumber == interlockCondition.ConditionNumber)
                .Update(entity);
        }

        /// <summary>
        /// 指定PLCのインターロック条件ビュー一覧を取得
        /// </summary>
        /// <param name="plcId">PLC ID</param>
        /// <returns>インターロック条件ビューリスト</returns>
        public async Task<List<ViewInterlockConditions>> GetViewInterlockConditionsByPlcIdAsync(int plcId)
        {
            var response = await _supabaseClient
                .From<ViewInterlockConditionsEntity>()
                .Where(vic => vic.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        #endregion

        #region InterlockIO Methods

        /// <summary>
        /// 指定シリンダーのインターロックIO一覧を取得
        /// </summary>
        /// <param name="cylinderId">シリンダーID</param>
        /// <returns>インターロックIOリスト</returns>
        public async Task<List<InterlockIO>> GetInterlockIOsByCylinderIdAsync(int cylinderId)
        {
            var response = await _supabaseClient
                .From<InterlockIOEntity>()
                .Where(i => i.CylinderId == cylinderId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        /// <summary>
        /// 指定IOアドレスとPLCのインターロックIO一覧を取得
        /// </summary>
        /// <param name="ioAddress">IOアドレス</param>
        /// <param name="plcId">PLC ID</param>
        /// <returns>インターロックIOリスト</returns>
        public async Task<List<InterlockIO>> GetIOInterlocksAsync(string ioAddress, int plcId)
        {
            var response = await _supabaseClient
                .From<InterlockIOEntity>()
                .Where(i => i.IOAddress == ioAddress)
                .Where(i => i.PlcId == plcId)
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        /// <summary>
        /// インターロックIOを追加
        /// </summary>
        /// <param name="interlockIO">インターロックIO</param>
        public async Task AddInterlockIOAssociationAsync(InterlockIO interlockIO)
        {
            await _supabaseClient
                .From<InterlockIOEntity>()
                .Insert(InterlockIOEntity.FromDto(interlockIO));
        }

        /// <summary>
        /// インターロックIOを削除
        /// </summary>
        /// <param name="interlockIO">削除対象のインターロックIO</param>
        public async Task DeleteInterlockIOAsync(InterlockIO interlockIO)
        {
            Debug.WriteLine($"[DeleteInterlockIOAsync] 削除開始");
            Debug.WriteLine($"  CylinderId: {interlockIO.CylinderId}");
            Debug.WriteLine($"  PlcId: {interlockIO.PlcId}");
            Debug.WriteLine($"  IOAddress: {interlockIO.IOAddress}");
            Debug.WriteLine($"  InterlockSortId: {interlockIO.InterlockSortId}");
            Debug.WriteLine($"  ConditionNumber: {interlockIO.ConditionNumber}");

            try
            {
                // 削除前に対象レコードが存在するか確認
                var existingRecords = await _supabaseClient
                    .From<InterlockIOEntity>()
                    .Filter("CylinderId", Operator.Equals, interlockIO.CylinderId.ToString())
                    .Filter("PlcId", Operator.Equals, interlockIO.PlcId.ToString())
                    .Filter("IOAddress", Operator.Equals, interlockIO.IOAddress)
                    .Filter("InterlockSortId", Operator.Equals, interlockIO.InterlockSortId.ToString())
                    .Filter("ConditionNumber", Operator.Equals, interlockIO.ConditionNumber.ToString())
                    .Get();

                Debug.WriteLine($"  削除前のレコード数: {existingRecords?.Models?.Count ?? 0}");

                // 削除実行 - Filterを使用
                await _supabaseClient
                    .From<InterlockIOEntity>()
                    .Filter("CylinderId", Operator.Equals, interlockIO.CylinderId.ToString())
                    .Filter("PlcId", Operator.Equals, interlockIO.PlcId.ToString())
                    .Filter("IOAddress", Operator.Equals, interlockIO.IOAddress)
                    .Filter("InterlockSortId", Operator.Equals, interlockIO.InterlockSortId.ToString())
                    .Filter("ConditionNumber", Operator.Equals, interlockIO.ConditionNumber.ToString())
                    .Delete();

                Debug.WriteLine($"  削除完了");

                // 削除後に確認
                var afterRecords = await _supabaseClient
                    .From<InterlockIOEntity>()
                    .Filter("CylinderId", Operator.Equals, interlockIO.CylinderId.ToString())
                    .Filter("PlcId", Operator.Equals, interlockIO.PlcId.ToString())
                    .Filter("IOAddress", Operator.Equals, interlockIO.IOAddress)
                    .Filter("InterlockSortId", Operator.Equals, interlockIO.InterlockSortId.ToString())
                    .Filter("ConditionNumber", Operator.Equals, interlockIO.ConditionNumber.ToString())
                    .Get();

                Debug.WriteLine($"  削除後のレコード数: {afterRecords?.Models?.Count ?? 0}");

                if (afterRecords?.Models?.Any() == true)
                {
                    Debug.WriteLine($"  警告: 削除後もレコードが残っています！");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[DeleteInterlockIOAsync] エラー: {ex.Message}");
                throw;
            }
        }

        /// <summary>
        /// インターロックIOを更新
        /// </summary>
        /// <param name="interlockIO">更新対象のインターロックIO</param>
        public async Task UpdateInterlockIOAsync(InterlockIO interlockIO)
        {
            var entity = InterlockIOEntity.FromDto(interlockIO);
            await _supabaseClient
                .From<InterlockIOEntity>()
                .Where(e => e.CylinderId == interlockIO.CylinderId)
                .Where(e => e.PlcId == interlockIO.PlcId)
                .Where(e => e.IOAddress == interlockIO.IOAddress)
                .Where(e => e.InterlockSortId == interlockIO.InterlockSortId)
                .Where(e => e.ConditionNumber == interlockIO.ConditionNumber)
                .Update(entity);
        }

        #endregion

        #region Interlock Master Data Methods

        /// <summary>
        /// インターロック条件タイプ一覧を取得
        /// </summary>
        /// <returns>インターロック条件タイプリスト</returns>
        public async Task<List<InterlockConditionType>> GetInterlockConditionTypesAsync()
        {
            var response = await _supabaseClient
                .From<InterlockConditionTypeEntity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        /// <summary>
        /// インターロック前提条件1一覧を取得
        /// </summary>
        /// <returns>インターロック前提条件1リスト</returns>
        public async Task<List<InterlockPrecondition1>> GetInterlockPrecondition1ListAsync()
        {
            var response = await _supabaseClient
                .From<InterlockPrecondition1Entity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        /// <summary>
        /// インターロック前提条件1を一括で追加または更新
        /// </summary>
        /// <param name="preconditions">前提条件1リスト</param>
        public async Task UpsertInterlockPrecondition1ListAsync(List<InterlockPrecondition1> preconditions)
        {
            foreach (var precondition in preconditions)
            {
                var entity = InterlockPrecondition1Entity.FromDto(precondition);
                await _supabaseClient
                    .From<InterlockPrecondition1Entity>()
                    .Upsert(entity);
            }
        }

        /// <summary>
        /// インターロック前提条件2一覧を取得
        /// </summary>
        /// <returns>インターロック前提条件2リスト</returns>
        public async Task<List<InterlockPrecondition2>> GetInterlockPrecondition2ListAsync()
        {
            var response = await _supabaseClient
                .From<InterlockPrecondition2Entity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        /// <summary>
        /// インターロック前提条件2を一括で追加または更新
        /// </summary>
        /// <param name="preconditions">前提条件2リスト</param>
        public async Task UpsertInterlockPrecondition2ListAsync(List<InterlockPrecondition2> preconditions)
        {
            foreach (var precondition in preconditions)
            {
                var entity = InterlockPrecondition2Entity.FromDto(precondition);
                await _supabaseClient
                    .From<InterlockPrecondition2Entity>()
                    .Upsert(entity);
            }
        }

        /// <summary>
        /// インターロック前提条件3一覧を取得
        /// 特定IOまたは特定デバイスがONしている場合にインターロックが有効になる条件
        /// </summary>
        /// <returns>インターロック前提条件3リスト</returns>
        public async Task<List<InterlockPrecondition3>> GetInterlockPrecondition3ListAsync()
        {
            var response = await _supabaseClient
                .From<InterlockPrecondition3Entity>()
                .Get();
            return response.Models.Select(e => e.ToDto()).ToList();
        }

        /// <summary>
        /// インターロック前提条件3を一括で追加または更新
        /// </summary>
        /// <param name="preconditions">前提条件3リスト</param>
        public async Task UpsertInterlockPrecondition3ListAsync(List<InterlockPrecondition3> preconditions)
        {
            foreach (var precondition in preconditions)
            {
                var entity = InterlockPrecondition3Entity.FromDto(precondition);
                await _supabaseClient
                    .From<InterlockPrecondition3Entity>()
                    .Upsert(entity);
            }
        }

        /// <summary>
        /// インターロック前提条件3を追加
        /// </summary>
        /// <param name="precondition">追加する前提条件3</param>
        /// <returns>追加された前提条件3（IDが設定されたもの）</returns>
        public async Task<InterlockPrecondition3> AddInterlockPrecondition3Async(InterlockPrecondition3 precondition)
        {
            var entity = InterlockPrecondition3Entity.FromDto(precondition);
            var response = await _supabaseClient
                .From<InterlockPrecondition3Entity>()
                .Insert(entity);
            return response.Models.First().ToDto();
        }

        /// <summary>
        /// インターロック前提条件3を更新
        /// </summary>
        /// <param name="precondition">更新する前提条件3</param>
        public async Task UpdateInterlockPrecondition3Async(InterlockPrecondition3 precondition)
        {
            var entity = InterlockPrecondition3Entity.FromDto(precondition);
            await _supabaseClient
                .From<InterlockPrecondition3Entity>()
                .Where(e => e.Id == precondition.Id)
                .Update(entity);
        }

        /// <summary>
        /// インターロック前提条件3を削除
        /// </summary>
        /// <param name="id">削除する前提条件3のID</param>
        public async Task DeleteInterlockPrecondition3Async(int id)
        {
            await _supabaseClient
                .From<InterlockPrecondition3Entity>()
                .Where(e => e.Id == id)
                .Delete();
        }

        #endregion
    }
}
