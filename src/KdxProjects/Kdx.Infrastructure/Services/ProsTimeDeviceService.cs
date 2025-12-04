using Kdx.Contracts.DTOs;
using Kdx.Contracts.Enums;
using Kdx.Contracts.Interfaces;
using Kdx.Infrastructure.Supabase.Repositories;
using System.Diagnostics;

namespace Kdx.Infrastructure.Services
{
    /// <summary>
    /// ProsTime（工程時間）デバイスの管理サービス実装
    /// Infrastructure層に配置することで、ビジネスロジックをUI層から分離
    /// </summary>
    public class ProsTimeDeviceService : IProsTimeDeviceService
    {
        private readonly ISupabaseRepository _repository;
        private readonly Dictionary<int, OperationProsTimeConfig> _loadedOperationConfigs;

        public class OperationProsTimeConfig
        {
            public int TotalProsTimeCount { get; set; }
            public Dictionary<int, int> SortIdToCategoryIdMap { get; set; }
            public OperationProsTimeConfig()
            {
                SortIdToCategoryIdMap = new Dictionary<int, int>();
            }
        }

        // デフォルト設定
        private static readonly OperationProsTimeConfig _defaultOperationConfig =
            new() { TotalProsTimeCount = 0, SortIdToCategoryIdMap = new Dictionary<int, int>() };

        public ProsTimeDeviceService(ISupabaseRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
            _loadedOperationConfigs = LoadOperationProsTimeConfigsFromDb();
        }

        /// <summary>
        /// ProsTimeテーブルの全レコードを削除する
        /// </summary>
        public void DeleteProsTimeTable()
        {
            Task.Run(async () => await _repository.DeleteProsTimeTableAsync()).GetAwaiter().GetResult();
        }

        public List<ProsTime> GetProsTimeByPlcId(int plcId)
        {
            return Task.Run(async () => await _repository.GetProsTimeByPlcIdAsync(plcId)).GetAwaiter().GetResult();
        }

        public List<ProsTime> GetProsTimeByMnemonicId(int plcId, int mnemonicId)
        {
            return Task.Run(async () => await _repository.GetProsTimeByMnemonicIdAsync(plcId, mnemonicId)).GetAwaiter().GetResult();
        }

        private Dictionary<int, OperationProsTimeConfig> LoadOperationProsTimeConfigsFromDb()
        {
            var configs = new Dictionary<int, OperationProsTimeConfig>();

            try
            {
                var rawConfigData = Task.Run(async () => await _repository.GetProsTimeDefinitionsAsync()).GetAwaiter().GetResult();

                if (rawConfigData == null || !rawConfigData.Any())
                {
                    // ProsTimeDefinitionsテーブルから設定を読み込めませんでした
                    return configs; // 空のコンフィグを返す
                }

                var groupedData = rawConfigData.GroupBy(r => r.OperationCategoryId);

                foreach (var group in groupedData)
                {
                    var operationCategoryKey = group.Key;
                    var totalCount = group.Count();

                    var map = new Dictionary<int, int>();
                    foreach (var item in group)
                    {
                        map[item.SortOrder] = item.OperationDefinitionsId;
                    }

                    configs[operationCategoryKey] = new OperationProsTimeConfig
                    {
                        TotalProsTimeCount = totalCount,
                        SortIdToCategoryIdMap = map
                    };
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading ProsTime definitions: {ex.Message}");
                return GetDefaultConfigs();
            }

            if (!configs.Any())
            {
                return GetDefaultConfigs();
            }

            return configs;
        }

        private Dictionary<int, OperationProsTimeConfig> GetDefaultConfigs()
        {
            var configs = new Dictionary<int, OperationProsTimeConfig>();

            // 各CategoryIdに対してデフォルト設定を生成（5個のProsTime）
            for (int categoryId = 1; categoryId <= 20; categoryId++)
            {
                configs[categoryId] = new OperationProsTimeConfig
                {
                    TotalProsTimeCount = 5,
                    SortIdToCategoryIdMap = new Dictionary<int, int>
                    {
                        {0, 1}, {1, 2}, {2, 3}, {3, 4}, {4, 5}
                    }
                };
            }

            return configs;
        }

        public void SaveProsTime(List<Operation> operations, int startCurrent, int startPrevious, int startCylinder, int plcId)
        {
            operations.Sort((o1, o2) => o1.Id.CompareTo(o2.Id));

            var allExistingProsTimesRaw = GetProsTimeByMnemonicId(plcId, (int)MnemonicType.Operation);
            var existingProsTimeMap = allExistingProsTimesRaw
                .GroupBy(pt => pt.RecordId)
                .ToDictionary(
                    g => g.Key,
                    g => g.ToDictionary(pt => pt.SortId, pt => pt)
                );

            var prosTimeDifinitions = Task.Run(async () => await _repository.GetProsTimeDefinitionsAsync()).GetAwaiter().GetResult();
            var cylinders = Task.Run(async () => await _repository.GetCYsAsync()).GetAwaiter().GetResult().Where(c => c.PlcId == plcId).ToList();

            var prosTimesToSave = new List<ProsTime>();
            int count = 0;
            List<Memory> currentMemories = new List<Memory>();
            List<Memory> previousMemories = new List<Memory>();
            List<Memory> cylinderMemories = new List<Memory>();


            foreach (Operation operation in operations)
            {
                if (operation == null || operation.CategoryId == null) continue;

                var operationCategoryValue = operation.CategoryId.Value;

                OperationProsTimeConfig currentConfig = _loadedOperationConfigs.TryGetValue(operationCategoryValue, out var specificConfig)
                                                        ? specificConfig
                                                        : _defaultOperationConfig;

                int prosTimeCount = currentConfig.TotalProsTimeCount;

                // 指定されたOperationのCategoryIdに対応するProsTimeDefinitionsを取得
                // ProsTimeDefinitionsにはカテゴリごとに設定が含まれている
                var prosTimeDefinitionByCategory = prosTimeDifinitions
                    .Where(d => d.OperationCategoryId == operationCategoryValue)
                    .OrderBy(d => d.SortOrder)
                    .ToList();

                foreach (ProsTimeDefinitions definition in prosTimeDefinitionByCategory)
                {
                    string currentDevice = "ZR" + (startCurrent + count).ToString();
                    string previousDevice = "ZR" + (startPrevious + count).ToString();
                    string cylinderDevice = "ZR" + (startCylinder + count).ToString();

                    ProsTime prosTime = new ProsTime
                    {
                        PlcId = plcId,
                        MnemonicId = (int)MnemonicType.Operation,
                        RecordId = operation.Id,
                        SortId = definition.SortOrder,
                        CurrentDevice = currentDevice,
                        PreviousDevice = previousDevice,
                        CylinderDevice = cylinderDevice,
                        CategoryId = operation.CategoryId ?? 0
                    };

                    var cylinder = cylinders.FirstOrDefault(c => c.Id == operation.CYId);
                    string row2 = cylinder != null ? cylinder.CYNum ?? "NaN" : "NaN";
                    string row3 = definition.Comment1 ?? "";
                    string row4 = definition.Comment2 ?? "";

                    Memory currentMemory = new()
                    {
                        PlcId = plcId,
                        Device = currentDevice,
                        MemoryCategory = 5,     // ZR
                        DeviceNumber = startCurrent + count,
                        DeviceNumber1 = (startPrevious + count).ToString(),
                        DeviceNumber2 = "",
                        Category = "工程ﾀｲﾑ",
                        Row_1 = "ﾀｲﾑ現在",
                        Row_2 = row2,
                        Row_3 = row3,
                        Row_4 = row4,
                        Direct_Input = "",
                        Confirm = "",
                        Note = "",
                        GOT = "true",
                        MnemonicId = (int)MnemonicType.Operation,
                        RecordId = operation.Id,
                        OutcoilNumber = 0
                    };

                    Memory previousMemory = new()
                    {
                        PlcId = plcId,
                        Device = previousDevice,
                        MemoryCategory = 5,     // ZR
                        DeviceNumber = startPrevious + count,
                        DeviceNumber1 = (startCurrent + count).ToString(),
                        DeviceNumber2 = "",
                        Category = "前工程ﾀｲﾑ",
                        Row_1 = "ﾀｲﾑ前回",
                        Row_2 = row2,
                        Row_3 = row3,
                        Row_4 = row4,
                        Direct_Input = "",
                        Confirm = "",
                        Note = "",
                        GOT = "true",
                        MnemonicId = (int)MnemonicType.Operation,
                        RecordId = operation.Id,
                        OutcoilNumber = 0
                    };

                    Memory cylinderMemory = new()
                    {
                        PlcId = plcId,
                        Device = cylinderDevice,
                        MemoryCategory = 5,     // ZR
                        DeviceNumber = startCylinder + count,
                        DeviceNumber1 = "",
                        DeviceNumber2 = "",
                        Category = "ｼﾘﾝﾀﾞ",
                        Row_1 = "CYﾀｲﾑ",
                        Row_2 = row2,
                        Row_3 = row3,
                        Row_4 = row4,
                        Direct_Input = "",
                        Confirm = "",
                        Note = "",
                        GOT = "true",
                        MnemonicId = (int)MnemonicType.Operation,
                        RecordId = operation.Id,
                        OutcoilNumber = 0
                    };

                    currentMemories.Add(currentMemory);
                    previousMemories.Add(previousMemory);
                    cylinderMemories.Add(cylinderMemory);
                    prosTimesToSave.Add(prosTime);

                    count++;

                }
            }

            // 重複を除去（PlcId, MnemonicId, RecordId, SortIdの組み合わせでユニークにする）
            var uniqueProsTimes = prosTimesToSave
                .GroupBy(pt => new { pt.PlcId, pt.MnemonicId, pt.RecordId, pt.SortId })
                .Select(g => g.First())
                .ToList();

            // 一括で保存
            if (uniqueProsTimes.Any())
            {
                Task.Run(async () => await _repository.SaveOrUpdateProsTimesBatchAsync(uniqueProsTimes)).GetAwaiter().GetResult();
                Task.Run(async () => await _repository.SaveOrUpdateMemoriesBatchAsync(currentMemories)).GetAwaiter().GetResult();
                Task.Run(async () => await _repository.SaveOrUpdateMemoriesBatchAsync(previousMemories)).GetAwaiter().GetResult();
                Task.Run(async () => await _repository.SaveOrUpdateMemoriesBatchAsync(cylinderMemories)).GetAwaiter().GetResult();
            }
        }
    }
}
