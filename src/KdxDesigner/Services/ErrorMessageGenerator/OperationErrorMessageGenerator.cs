using Kdx.Contracts.DTOs;
using Kdx.Contracts.Enums;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.Models;

namespace KdxDesigner.Services.ErrorMessageGenerator
{
    /// <summary>
    /// Operation用エラーメッセージ生成サービス
    /// メモリストアのデータを元にGeneratedErrorを生成する
    /// </summary>
    public class OperationErrorMessageGenerator : ErrorMessageGeneratorBase, IOperationErrorMessageGenerator
    {
        public OperationErrorMessageGenerator(ISupabaseRepository repository) : base(repository)
        {
        }

        /// <summary>
        /// Operation用エラーメッセージを生成
        /// 1. OperationとCategoryIdからProcessErrorを生成
        /// 2. ProcessErrorとOperationからOperationErrorInputを構築
        /// 3. OperationErrorInputからGeneratedErrorを生成
        /// </summary>
        public async Task<List<GeneratedError>> GenerateOperationErrorsAsync(
            List<Operation> operations,
            List<IO> ioList,
            int plcId,
            int cycleId,
            int startErrorNum,
            int deviceStartM,
            int deviceStartT)
        {
            var errors = new List<GeneratedError>();
            var processErrors = new List<ProcessError>();
            var messages = await _repository.GetErrorMessagesAsync((int)MnemonicType.Operation);

            // ステップ1: OperationとCategoryIdからProcessErrorを生成
            foreach (var operation in operations)
            {
                if (operation == null || operation.CategoryId == null)
                {
                    continue;
                }

                // CategoryIdに応じたAlarmIdとSpeedNumberのリストを取得
                var alarmInfos = GetAlarmIdsForCategory(operation.CategoryId);

                // 各OperationごとにAlarmCountを0からカウント
                int alarmCount = 0;
                foreach (var (alarmId, speedNumber) in alarmInfos)
                {
                    // ProcessErrorを作成（AlarmId情報を含む）
                    var processError = new ProcessError
                    {
                        PlcId = plcId,
                        CycleId = cycleId,
                        MnemonicId = (int)MnemonicType.Operation,
                        RecordId = operation.Id,
                        AlarmId = alarmId,
                        AlarmCount = alarmCount++,
                        SpeedNumber = speedNumber
                        // Device, ErrorNum等は後で設定
                    };

                    processErrors.Add(processError);
                }
            }

            // ステップ2: ProcessErrorとOperationからOperationErrorInputを構築
            var operationInputs = BuildOperationErrorInputsFromOperations(
                processErrors,
                operations,
                ioList,
                plcId,
                cycleId);

            // ステップ3: OperationErrorInputからGeneratedErrorを生成
            int errorNum = startErrorNum;

            foreach (var input in operationInputs)
            {
                // AlarmIdに対応するエラーメッセージテンプレートを取得
                var msg = messages.FirstOrDefault(m => m.AlarmId == input.AlarmId);
                if (msg == null)
                {
                    System.Diagnostics.Debug.WriteLine($"AlarmId {input.AlarmId} に対応するエラーメッセージが見つかりません。");
                    continue;
                }

                // プレースホルダー置換用辞書を構築
                var replacements = BuildOperationReplacements(input);

                // IO情報を収集
                var (ioAddresses, ioNames, ioConditions) = CollectOperationIoInfo(input);

                var deviceM = $"M{deviceStartM + errorNum}";
                var deviceT = $"T{deviceStartT + errorNum}";

                // ProcessErrorを更新（デバイス情報とエラー番号を設定）
                // AlarmCountで一意に識別（同一OperationId + AlarmIdが複数存在する場合があるため）
                var processError = processErrors.FirstOrDefault(pe =>
                    pe.RecordId == input.OperationId &&
                    pe.AlarmCount == input.AlarmCount);

                if (processError != null)
                {
                    processError.Device = deviceM;
                    processError.ErrorNum = errorNum;
                    processError.ErrorTimeDevice = deviceT;
                    processError.Comment1 = ReplacePlaceholders(msg.Category1, replacements);
                    processError.Comment2 = $"{input.Valve1}{input.GoBack}";
                    processError.Comment3 = ReplacePlaceholders(msg.Category2, replacements);
                    processError.Comment4 = ReplacePlaceholders(msg.Category3, replacements);
                    processError.AlarmComment = ReplacePlaceholders(msg.BaseAlarm, replacements);
                    processError.MessageComment = ReplacePlaceholders(msg.BaseMessage, replacements);
                    processError.ErrorTime = msg.DefaultCountTime;
                    processError.IoAddresses = ioAddresses;
                    processError.IoNames = ioNames;
                    processError.IoConditions = ioConditions;
                }

                // ProcessErrorからGeneratedErrorを生成
                var error = new GeneratedError
                {
                    PlcId = plcId,
                    ErrorNum = errorNum,
                    MnemonicId = (int)MnemonicType.Operation,
                    AlarmId = input.AlarmId,
                    RecordId = input.OperationId,
                    DeviceM = deviceM,
                    DeviceT = deviceT,
                    Comment1 = ReplacePlaceholders(msg.Category1, replacements),
                    Comment2 = $"{input.Valve1}{input.GoBack}",
                    Comment3 = ReplacePlaceholders(msg.Category2, replacements),
                    Comment4 = ReplacePlaceholders(msg.Category3, replacements),
                    AlarmComment = ReplacePlaceholders(msg.BaseAlarm, replacements),
                    MessageComment = ReplacePlaceholders(msg.BaseMessage, replacements),
                    ErrorTime = msg.DefaultCountTime,
                    CycleId = cycleId
                };

                errors.Add(error);
                errorNum++;
            }

            // ProcessErrorをデータベースに保存
            if (processErrors.Count > 0)
            {
                await _repository.SaveErrorsAsync(processErrors);
            }

            return errors;
        }

        /// <summary>
        /// Operation用のIO情報を収集
        /// </summary>
        /// <param name="input">Operationエラー入力データ</param>
        /// <returns>(IoAddresses, IoNames, IoConditions)のタプル</returns>
        private static (string?, string?, string?) CollectOperationIoInfo(OperationErrorInput input)
        {
            var addresses = new List<string>();
            var names = new List<string>();
            var conditions = new List<string>();

            // Start IOを追加
            foreach (var io in input.StartIOs)
            {
                addresses.Add(io.Address);
                if (!string.IsNullOrEmpty(io.IOName))
                {
                    names.Add(io.IOName);
                    var condition = io.IsOnCondition ? "ON" : "OFF";
                    conditions.Add($"{io.IOName}:{condition}");
                }
            }

            // Finish IOを追加
            foreach (var io in input.FinishIOs)
            {
                addresses.Add(io.Address);
                if (!string.IsNullOrEmpty(io.IOName))
                {
                    names.Add(io.IOName);
                    var condition = io.IsOnCondition ? "ON" : "OFF";
                    conditions.Add($"{io.IOName}:{condition}");
                }
            }

            // Speed IOを追加
            foreach (var io in input.SpeedIOs)
            {
                addresses.Add(io.Address);
                if (!string.IsNullOrEmpty(io.IOName))
                {
                    names.Add(io.IOName);
                    var condition = io.IsOnCondition ? "ON" : "OFF";
                    conditions.Add($"{io.IOName}:{condition}");
                }
            }

            // Con IOを追加
            if (input.ConIO != null)
            {
                addresses.Add(input.ConIO.Address);
                if (!string.IsNullOrEmpty(input.ConIO.IOName))
                {
                    names.Add(input.ConIO.IOName);
                    var condition = input.ConIO.IsOnCondition ? "ON" : "OFF";
                    conditions.Add($"{input.ConIO.IOName}:{condition}");
                }
            }

            var ioAddresses = addresses.Any() ? string.Join(",", addresses) : null;
            var ioNames = names.Any() ? string.Join(",", names) : null;
            var ioConditions = conditions.Any() ? string.Join(", ", conditions) : null;

            return (ioAddresses, ioNames, ioConditions);
        }

        /// <summary>
        /// Operation用プレースホルダー辞書を構築
        /// </summary>
        /// <param name="input">Operationエラー入力データ</param>
        /// <returns>プレースホルダー置換用辞書</returns>
        private static Dictionary<string, string?> BuildOperationReplacements(OperationErrorInput input)
        {
            var replacements = new Dictionary<string, string?>
            {
                { "OperationName", input.OperationName },
                { "Valve1", input.Valve1 },
                { "Valve2", input.Valve2 },
                { "GoBack", input.GoBack },
                { "CategoryName", input.CategoryName },
                // IO条件の表示文字列
                { "StartCondition", input.StartConditionDisplay },
                { "FinishCondition", input.FinishConditionDisplay },
                { "SpeedCondition", input.SpeedConditionDisplay },
                { "ConIO", input.ConIO?.DisplayCondition ?? "" },
                // SpeedNumber関連のプレースホルダー
                { "SpeedNumber", input.SpeedNumber?.ToString() ?? "" },
                { "SpeedSensorName", input.SpeedSensorName ?? "" },
                { "SpeedSensorAddress", input.SpeedSensorAddress ?? "" },
                { "SpeedSensorExplain", input.SpeedSensorExplain ?? "" }
            };

            // Start IO情報プレースホルダーを追加
            AddOperationIoPlaceholders(replacements, input.StartIOs, "StartIO");

            // Finish IO情報プレースホルダーを追加
            AddOperationIoPlaceholders(replacements, input.FinishIOs, "FinishIO");

            // Speed IO情報プレースホルダーを追加
            AddOperationIoPlaceholders(replacements, input.SpeedIOs, "SpeedIO");

            // Con IO情報プレースホルダーを追加（単一）
            if (input.ConIO != null)
            {
                replacements.Add("ConIO.Address", input.ConIO.Address);
                replacements.Add("ConIO.IOName", input.ConIO.IOName ?? "");
                replacements.Add("ConIO.IOExplanation", input.ConIO.IOExplanation ?? "");
            }
            else
            {
                replacements.Add("ConIO.Address", "");
                replacements.Add("ConIO.IOName", "");
                replacements.Add("ConIO.IOExplanation", "");
            }

            return replacements;
        }

        /// <summary>
        /// Operation用のIO情報プレースホルダーを辞書に追加
        /// </summary>
        /// <param name="replacements">プレースホルダー辞書</param>
        /// <param name="ioInfoList">IO情報リスト</param>
        /// <param name="prefix">プレースホルダーのプレフィックス（"StartIO", "FinishIO", "SpeedIO"）</param>
        private static void AddOperationIoPlaceholders(Dictionary<string, string?> replacements, List<OperationIoInfo> ioInfoList, string prefix)
        {
            // 最初のIOの情報を追加（後方互換性のため）
            var firstIo = ioInfoList.FirstOrDefault();
            if (firstIo != null)
            {
                replacements.Add($"{prefix}.Address", firstIo.Address);
                replacements.Add($"{prefix}.IOName", firstIo.IOName ?? "");
                replacements.Add($"{prefix}.IOExplanation", firstIo.IOExplanation ?? "");
                replacements.Add($"{prefix}.DisplayCondition", firstIo.DisplayCondition);
            }
            else
            {
                replacements.Add($"{prefix}.Address", "");
                replacements.Add($"{prefix}.IOName", "");
                replacements.Add($"{prefix}.IOExplanation", "");
                replacements.Add($"{prefix}.DisplayCondition", "");
            }

            // インデックス付きIO情報プレースホルダーを追加 (例: {StartIO[0].Address})
            for (int i = 0; i < ioInfoList.Count; i++)
            {
                var io = ioInfoList[i];
                replacements.Add($"{prefix}[{i}].Address", io.Address);
                replacements.Add($"{prefix}[{i}].IOName", io.IOName ?? "");
                replacements.Add($"{prefix}[{i}].IOExplanation", io.IOExplanation ?? "");
                replacements.Add($"{prefix}[{i}].DisplayCondition", io.DisplayCondition);
            }
        }

        /// <summary>
        /// カテゴリIDに応じたAlarmIdとSpeedNumberのリストを取得
        /// (既存のErrorService.SaveMnemonicDeviceOperationのロジックを踏襲)
        /// SpeedNumber: AlarmId=3,4の場合に速度センサー番号(1=SS1, 2=SS2, ...)を設定
        /// </summary>
        private static List<(int AlarmId, int? SpeedNumber)> GetAlarmIdsForCategory(int? categoryId)
        {
            return categoryId switch
            {
                2 or 29 or 30 => [(1, null), (2, null), (5, null)], // 保持
                3 or 9 or 15 or 27 => [(1, null), (2, null), (5, null)], // 速度制御INV1
                4 or 10 or 16 or 28 => [(1, null), (2, null), (3, 1), (4, 1), (5, null)], // 速度制御INV2
                5 or 11 or 17 => [(1, null), (2, null), (3, 1), (4, 1), (3, 2), (4, 2), (5, null)], // 速度制御INV3
                6 or 12 or 18 => [(1, null), (2, null), (3, 1), (4, 1), (3, 2), (4, 2), (3, 3), (4, 3), (5, null)], // 速度制御INV4
                7 or 13 or 19 => [(1, null), (2, null), (3, 1), (4, 1), (3, 2), (4, 2), (3, 3), (4, 3), (3, 4), (4, 4), (5, null)], // 速度制御INV5
                20 => [(5, null)], // バネ
                31 => [], // サーボ
                _ => []
            };
        }

        /// <summary>
        /// OperationリストからOperation入力データを生成
        /// </summary>
        /// <param name="operations">Operationリスト</param>
        /// <param name="ioList">IOリスト</param>
        /// <param name="plcId">PLC ID</param>
        /// <param name="cycleId">Cycle ID</param>
        /// <returns>OperationErrorInputリスト</returns>
        public List<OperationErrorInput> BuildOperationErrorInputsFromOperations(
            List<ProcessError> errors,
            List<Operation> operations,
            List<IO> ioList,
            int plcId,
            int cycleId)
        {
            var inputs = new List<OperationErrorInput>();

            foreach (var error in errors)
            {
                if (error == null)
                {
                    continue;
                }

                var operation = operations.FirstOrDefault(op => op.Id == error.RecordId);

                if (operation == null)
                {
                    continue;
                }

                // Start条件のIOを取得
                var startIOs = FindIOs(ioList, operation.Start);

                // Finish条件のIOを取得
                var finishIOs = FindIOs(ioList, operation.Finish);

                // 制御センサーのIOを取得
                var conIO = FindIOs(ioList, operation.Con).FirstOrDefault();

                // 速度センサーのIOを取得（ProcessErrorのSpeedNumberに基づいて単一のセンサーを取得）
                var speedIOs = GetSpeedSensorIOs(ioList, operation, error.SpeedNumber);

                // SpeedNumber関連の情報を取得
                var speedNumber = error.SpeedNumber;
                var speedSensorName = speedNumber.HasValue ? $"SS{speedNumber}" : null;
                var firstSpeedIO = speedIOs.FirstOrDefault();

                var input = new OperationErrorInput
                {
                    OperationId = operation.Id,
                    OperationName = operation.OperationName,
                    AlarmId = error.AlarmId ?? 0,
                    AlarmCount = error.AlarmCount ?? 0,
                    SpeedNumber = speedNumber,
                    SpeedSensorName = speedSensorName,
                    SpeedSensorAddress = firstSpeedIO?.Address,
                    SpeedSensorExplain = firstSpeedIO?.IOExplanation,
                    CategoryId = operation.CategoryId,
                    CategoryName = GetCategoryName(operation.CategoryId),
                    Valve1 = operation.Valve1,
                    Valve2 = "", // Valve2は現在使用されていない
                    GoBack = operation.GoBack,
                    InputDevice = operation.Start,
                    OutputDevice = operation.Finish,
                    Device = "", // デバイスは生成時に計算される
                    DeviceNumber = 0, // デバイス番号は生成時に計算される
                    PlcId = plcId,
                    CycleId = cycleId,
                    StartIOs = startIOs,
                    FinishIOs = finishIOs,
                    ConIO = conIO,
                    SpeedIOs = speedIOs
                };

                if (input.SpeedNumber != null || input.SpeedSensorAddress != null)
                {
                    Console.WriteLine($"Operation ID {input.OperationId} uses SpeedNumber {input.SpeedNumber}.");
                    Console.WriteLine($"Speed Sensor IO Address: {input.SpeedSensorAddress}:{input.SpeedSensorName}:{input.SpeedSensorExplain}");
                }



                inputs.Add(input);
            }

            return inputs;
        }

        /// <summary>
        /// IOテキストに一致するIOリストを検索してOperationIoInfoに変換
        /// </summary>
        /// <param name="ioList">IOリスト</param>
        /// <param name="ioText">検索するIOテキスト（例: "G", "B"）</param>
        /// <returns>一致したOperationIoInfoリスト</returns>
        private static List<OperationIoInfo> FindIOs(List<IO> ioList, string? ioText)
        {
            if (string.IsNullOrEmpty(ioText) || ioList == null || ioList.Count == 0)
            {
                return new List<OperationIoInfo>();
            }

            // IOテキストが "_" で始まる場合はON条件、そうでない場合はOFF条件
            bool isOnCondition = ioText.StartsWith("_");
            string searchText = isOnCondition ? ioText.Substring(1) : ioText;

            // IONameに一致するIOを検索
            var matchedIOs = ioList
                .Where(io => !string.IsNullOrEmpty(io.IOName) && io.IOName.Contains(searchText))
                .Select((io, index) => new OperationIoInfo
                {
                    Index = index,
                    Address = io.Address ?? "",
                    IOName = io.IOName,
                    IOExplanation = io.IOExplanation,
                    XComment = io.XComment,
                    YComment = io.YComment,
                    FComment = io.FComment,
                    IOSpot = io.IOSpot,
                    UnitName = io.UnitName,
                    System = io.System,
                    StationNumber = io.StationNumber,
                    LinkDevice = io.LinkDevice,
                    IsOnCondition = isOnCondition
                })
                .ToList();

            return matchedIOs;
        }

        /// <summary>
        /// SpeedNumberに基づいて対応する速度センサーのIOを取得
        /// </summary>
        /// <param name="ioList">IOリスト</param>
        /// <param name="operation">Operation情報</param>
        /// <param name="speedNumber">速度センサー番号（1=SS1, 2=SS2, 3=SS3, 4=SS4, 5=SS5, null=速度センサーなし）</param>
        /// <returns>速度センサーのIOリスト</returns>
        private static List<OperationIoInfo> GetSpeedSensorIOs(List<IO> ioList, Operation operation, int? speedNumber)
        {
            if (speedNumber == null)
            {
                return new List<OperationIoInfo>();
            }

            // SpeedNumberに対応する速度センサーのIOを取得
            string? sensorValue = speedNumber switch
            {
                1 => operation.SS1,
                2 => operation.SS2,
                3 => operation.SS3,
                4 => operation.SS4,
                _ => null
            };

            if (string.IsNullOrEmpty(sensorValue))
            {
                return new List<OperationIoInfo>();
            }

            return FindIOs(ioList, sensorValue);
        }

        /// <summary>
        /// CategoryIdからカテゴリ名を取得
        /// </summary>
        private static string? GetCategoryName(int? categoryId)
        {
            return categoryId switch
            {
                2 or 29 or 30 => "保持",
                3 or 9 or 15 or 27 => "速度制御1",
                4 or 10 or 16 or 28 => "速度制御2",
                5 or 11 or 17 => "速度制御3",
                6 or 12 or 18 => "速度制御4",
                7 or 13 or 19 => "速度制御5",
                20 => "バネ",
                31 => "サーボ",
                _ => $"Category{categoryId}"
            };
        }
    }
}
