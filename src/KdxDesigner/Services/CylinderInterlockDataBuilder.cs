using Kdx.Contracts.DTOs;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.Models;

namespace KdxDesigner.Services
{
    /// <summary>
    /// CylinderInterlockDataを構築するビルダークラス
    /// </summary>
    public class CylinderInterlockDataBuilder
    {
        private readonly ISupabaseRepository _supabaseRepository;
        private readonly SupabaseRepository _supabaseRepositoryImpl;

        // キャッシュ用
        private Dictionary<int, InterlockPrecondition1>? _precondition1Cache;
        private Dictionary<int, InterlockPrecondition2>? _precondition2Cache;
        private Dictionary<int, InterlockConditionType>? _conditionTypeCache;
        private Dictionary<int, Cylinder>? _cylinderCache;
        private List<IO>? _ioCache;
        private int _cachedPlcId = -1;

        public CylinderInterlockDataBuilder(
            ISupabaseRepository supabaseRepository,
            SupabaseRepository supabaseRepositoryImpl)
        {
            _supabaseRepository = supabaseRepository;
            _supabaseRepositoryImpl = supabaseRepositoryImpl;
        }

        /// <summary>
        /// 指定されたPlcIdに紐づく全CylinderのInterlock関連データを取得して構築します
        /// </summary>
        /// <param name="plcId">対象のPLC ID</param>
        /// <returns>構築されたCylinderInterlockDataのリスト</returns>
        public async Task<List<CylinderInterlockData>> BuildByPlcIdAsync(int plcId)
        {
            // マスターデータのキャッシュを初期化
            await InitializeCachesAsync(plcId);

            // PlcIdに紐づくCylinderを取得
            var cylinders = await _supabaseRepository.GetCyListAsync(plcId);

            var results = new List<CylinderInterlockData>();
            foreach (var cylinder in cylinders)
            {
                var data = await BuildCylinderDataAsync(cylinder);
                results.Add(data);
            }

            return results;
        }

        /// <summary>
        /// 指定されたCylinderに紐づくInterlock関連データを全て取得して構築します
        /// </summary>
        /// <param name="cylinder">対象のCylinder</param>
        /// <returns>構築されたCylinderInterlockData</returns>
        public async Task<CylinderInterlockData> BuildAsync(Cylinder cylinder)
        {
            // マスターデータのキャッシュを初期化
            await InitializeCachesAsync(cylinder.PlcId);

            return await BuildCylinderDataAsync(cylinder);
        }

        /// <summary>
        /// 複数のCylinderに対してInterlock関連データを一括取得して構築します
        /// </summary>
        /// <param name="cylinders">対象のCylinderリスト</param>
        /// <returns>構築されたCylinderInterlockDataのリスト</returns>
        public async Task<List<CylinderInterlockData>> BuildAsync(IEnumerable<Cylinder> cylinders)
        {
            var cylinderList = cylinders.ToList();
            if (!cylinderList.Any())
            {
                return new List<CylinderInterlockData>();
            }

            // 最初のCylinderのPlcIdでキャッシュを初期化（同一PLCを想定）
            await InitializeCachesAsync(cylinderList.First().PlcId);

            var results = new List<CylinderInterlockData>();

            foreach (var cylinder in cylinderList)
            {
                var data = await BuildCylinderDataAsync(cylinder);
                results.Add(data);
            }

            return results;
        }

        private async Task<CylinderInterlockData> BuildCylinderDataAsync(Cylinder cylinder)
        {
            var result = new CylinderInterlockData
            {
                Cylinder = cylinder,
                Interlocks = new List<InterlockData>()
            };

            // CylinderIdに紐づくInterlockを取得
            var interlocks = await _supabaseRepositoryImpl.GetInterlocksByCylindrIdAsync(cylinder.Id);

            foreach (var interlock in interlocks)
            {
                var interlockData = await BuildInterlockDataAsync(interlock);
                result.Interlocks.Add(interlockData);
            }

            return result;
        }

        private async Task InitializeCachesAsync(int plcId)
        {
            // 同じPlcIdの場合はキャッシュを再利用
            if (_cachedPlcId == plcId && _precondition1Cache != null)
            {
                return;
            }

            _cachedPlcId = plcId;

            // Precondition1のキャッシュ
            var precondition1List = await _supabaseRepositoryImpl.GetInterlockPrecondition1ListAsync();
            _precondition1Cache = precondition1List.ToDictionary(p => p.Id);

            // Precondition2のキャッシュ
            var precondition2List = await _supabaseRepositoryImpl.GetInterlockPrecondition2ListAsync();
            _precondition2Cache = precondition2List.ToDictionary(p => p.Id);

            // ConditionTypeのキャッシュ
            var conditionTypes = await _supabaseRepositoryImpl.GetInterlockConditionTypesAsync();
            _conditionTypeCache = conditionTypes.ToDictionary(ct => ct.Id);

            // Cylinderのキャッシュ（条件シリンダー用）
            var cylinderList = await _supabaseRepository.GetCyListAsync(plcId);
            _cylinderCache = cylinderList.ToDictionary(c => c.Id);

            // IOのキャッシュ
            _ioCache = await _supabaseRepository.GetIoListAsync();
        }

        private async Task<InterlockData> BuildInterlockDataAsync(Interlock interlock)
        {
            var interlockData = new InterlockData
            {
                Interlock = interlock,
                Conditions = new List<InterlockConditionData>()
            };

            // Precondition1を設定
            if (interlock.PreConditionID1.HasValue && _precondition1Cache != null)
            {
                _precondition1Cache.TryGetValue(interlock.PreConditionID1.Value, out var precondition1);
                interlockData.Precondition1 = precondition1;
            }

            // Precondition2を設定
            if (interlock.PreConditionID2.HasValue && _precondition2Cache != null)
            {
                _precondition2Cache.TryGetValue(interlock.PreConditionID2.Value, out var precondition2);
                interlockData.Precondition2 = precondition2;
            }

            // 条件シリンダーを設定
            if (_cylinderCache != null)
            {
                _cylinderCache.TryGetValue(interlock.ConditionCylinderId, out var conditionCylinder);
                interlockData.ConditionCylinder = conditionCylinder;
            }

            // InterlockConditionを取得（DTOからInterlockConditionに変換）
            var conditionDTOs = await _supabaseRepositoryImpl.GetInterlockConditionsByCylinderIdAsync(interlock.CylinderId);
            var filteredConditions = conditionDTOs
                .Where(c => c.CylinderId == interlock.CylinderId && c.InterlockSortId == interlock.SortId)
                .Select(dto => new InterlockCondition
                {
                    CylinderId = dto.CylinderId,
                    ConditionNumber = dto.ConditionNumber,
                    InterlockSortId = dto.InterlockSortId,
                    ConditionTypeId = dto.ConditionTypeId,
                    Name = dto.Name,
                    Device = dto.Device,
                    IsOnCondition = dto.IsOnCondition,
                    Comment1 = dto.Comment1,
                    Comment2 = dto.Comment2
                })
                .ToList();

            foreach (var condition in filteredConditions)
            {
                var conditionData = await BuildConditionDataAsync(condition);
                interlockData.Conditions.Add(conditionData);
            }

            return interlockData;
        }

        private async Task<InterlockConditionData> BuildConditionDataAsync(InterlockCondition condition)
        {
            var conditionData = new InterlockConditionData
            {
                Condition = condition,
                IOs = new List<InterlockIOData>()
            };

            // ConditionTypeを設定
            if (condition.ConditionTypeId.HasValue && _conditionTypeCache != null)
            {
                _conditionTypeCache.TryGetValue(condition.ConditionTypeId.Value, out var conditionType);
                conditionData.ConditionType = conditionType;
            }

            // InterlockIOを取得
            var ios = await _supabaseRepositoryImpl.GetInterlockIOsByCylinderIdAsync(condition.CylinderId);
            var filteredIOs = ios
                .Where(io => io.CylinderId == condition.CylinderId
                          && io.InterlockSortId == condition.InterlockSortId
                          && io.ConditionNumber == condition.ConditionNumber)
                .ToList();

            foreach (var io in filteredIOs)
            {
                var ioData = BuildIOData(io);
                conditionData.IOs.Add(ioData);
            }

            return conditionData;
        }

        private InterlockIOData BuildIOData(InterlockIO io)
        {
            var ioData = new InterlockIOData
            {
                IO = io
            };

            // IOテーブルから詳細情報を設定
            if (_ioCache != null)
            {
                var ioInfo = _ioCache.FirstOrDefault(i => i.Address == io.IOAddress && i.PlcId == io.PlcId);
                // IODetail全体を設定（IOName, IOExplanation, XComment等すべての情報が利用可能に）
                ioData.IODetail = ioInfo;
            }

            return ioData;
        }

        /// <summary>
        /// キャッシュをクリアします
        /// </summary>
        public void ClearCache()
        {
            _precondition1Cache = null;
            _precondition2Cache = null;
            _conditionTypeCache = null;
            _cylinderCache = null;
            _ioCache = null;
        }
    }
}
