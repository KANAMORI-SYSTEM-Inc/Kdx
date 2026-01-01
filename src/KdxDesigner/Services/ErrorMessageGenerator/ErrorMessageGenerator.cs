using Kdx.Contracts.DTOs;
using Kdx.Contracts.Enums;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.Models;

namespace KdxDesigner.Services.ErrorMessageGenerator
{
    /// <summary>
    /// エラーメッセージ生成サービス
    /// メモリストアのデータを元にGeneratedErrorを生成する
    /// </summary>
    public class ErrorMessageGenerator : IErrorMessageGenerator
    {
        private readonly ISupabaseRepository _repository;

        // UI表示用の装飾記号（選択状態を示す●○マーク）
        // これらはエラーメッセージには不要なため置換時に削除
        private const string FILLED_CIRCLE = "●";
        private const string HOLLOW_CIRCLE = "○";

        // 全角・半角空白（エラーメッセージの整形用）
        private const string HALF_WIDTH_SPACE = " ";
        private const string FULL_WIDTH_SPACE = "　";

        // InterlockエラーInputのID計算用定数
        private const int INTERLOCK_ID_MULTIPLIER = 1000;
        private const int INTERLOCK_CONDITION_ID_BASE_MULTIPLIER = 100000;
        private const int INTERLOCK_CONDITION_ID_SORT_MULTIPLIER = 100;

        public ErrorMessageGenerator(ISupabaseRepository repository)
        {
            _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        /// <summary>
        /// Interlock用エラーメッセージを生成
        /// </summary>
        public async Task<List<GeneratedError>> GenerateInterlockErrorsAsync(
            List<InterlockErrorInput> inputs,
            int startErrorNum,
            int deviceStartM,
            int deviceStartT)
        {
            var errors = new List<GeneratedError>();

            // 全てのInterlock用メッセージを取得（ConditionTypeIdでグループ化するため）
            var allMessages = await _repository.GetAllErrorMessagesAsync();
            var interlockMessages = allMessages
                .Where(m => m.MnemonicId == (int)MnemonicType.Interlock)
                .ToList();

            int errorNum = startErrorNum;

            foreach (var input in inputs)
            {
                // プレースホルダー置換用辞書を構築
                var replacements = BuildInterlockReplacements(input);

                // ConditionTypeIdに基づいてメッセージを選択
                var messages = GetMessagesForConditionType(interlockMessages, input.ConditionTypeId);

                foreach (var msg in messages)
                {
                    var error = new GeneratedError
                    {
                        PlcId = input.PlcId,
                        ErrorNum = input.DeviceNumber,
                        MnemonicId = (int)MnemonicType.Interlock,
                        AlarmId = input.DeviceNumber,
                        RecordId = input.InterlockConditionId,
                        DeviceM = input.Device,
                        DeviceT = string.Empty, // インターロックはTデバイスを使用しない
                        Comment1 = ReplacePlaceholders(msg.Category1, replacements),
                        Comment2 = ReplacePlaceholders(msg.Category2, replacements),
                        Comment3 = ReplacePlaceholders(msg.Category3, replacements),
                        Comment4 = "",
                        AlarmComment = ReplacePlaceholders(msg.BaseAlarm, replacements),
                        MessageComment = ReplacePlaceholders(msg.BaseMessage, replacements),
                        ErrorTime = 0,  // インターロックはタイマーが無い、即時エラー発生
                        CycleId = null
                    };

                    errors.Add(error);
                    errorNum++;
                }
            }

            return errors;
        }

        /// <summary>
        /// Operation用エラーメッセージを生成
        /// </summary>
        public async Task<List<GeneratedError>> GenerateOperationErrorsAsync(
            List<OperationErrorInput> inputs,
            int startErrorNum,
            int deviceStartM,
            int deviceStartT)
        {
            var errors = new List<GeneratedError>();
            var messages = await _repository.GetErrorMessagesAsync((int)MnemonicType.Operation);

            int errorNum = startErrorNum;

            foreach (var input in inputs)
            {
                // カテゴリに応じたAlarmIdリストを取得
                var alarmIds = GetAlarmIdsForCategory(input.CategoryId);

                // プレースホルダー置換用辞書を構築
                var replacements = BuildOperationReplacements(input);

                foreach (var alarmId in alarmIds)
                {
                    var msg = messages.FirstOrDefault(m => m.AlarmId == alarmId);
                    if (msg == null)
                    {
                        continue;
                    }

                    var error = new GeneratedError
                    {
                        PlcId = input.PlcId,
                        ErrorNum = errorNum,
                        MnemonicId = (int)MnemonicType.Operation,
                        AlarmId = alarmId,
                        RecordId = input.OperationId,
                        DeviceM = $"M{deviceStartM + errorNum}",
                        DeviceT = $"T{deviceStartT + errorNum}",
                        Comment1 = ReplacePlaceholders(msg.Category1, replacements),
                        Comment2 = $"{input.Valve1}{input.GoBack}",
                        Comment3 = ReplacePlaceholders(msg.Category2, replacements),
                        Comment4 = ReplacePlaceholders(msg.Category3, replacements),
                        AlarmComment = ReplacePlaceholders(msg.BaseAlarm, replacements),
                        MessageComment = ReplacePlaceholders(msg.BaseMessage, replacements),
                        ErrorTime = msg.DefaultCountTime,
                        CycleId = input.CycleId
                    };

                    errors.Add(error);
                    errorNum++;
                }
            }

            return errors;
        }

        /// <summary>
        /// Interlock用プレースホルダー辞書を構築
        /// </summary>
        /// <param name="input">Interlockエラー入力データ</param>
        /// <returns>プレースホルダー置換用辞書</returns>
        private Dictionary<string, string?> BuildInterlockReplacements(InterlockErrorInput input)
        {
            var replacements = new Dictionary<string, string?>
            {
                { "CylinderName", input.CylinderName },
                { "GoBack", input.GoOrBackDisplayName },
                { "ConditionCylinderName", input.ConditionCylinderName },
                { "ConditionType", input.ConditionTypeName },
                { "ConditionName", input.ConditionName },
                { "ConditionDevice", input.ConditionDevice },
                { "Comment1", input.ConditionComment1 },
                { "Comment2", input.ConditionComment2 },
                { "ConditionNumber", input.ConditionNumber.ToString() },
                // 拡張フィールド
                { "Precondition1", input.Precondition1Info ?? "" },
                { "Precondition2", input.Precondition2Info ?? "" },
                { "IOConditions", input.IOConditions.Count > 0 ? string.Join(", ", input.IOConditions) : "" },
                { "DetailedIOConditions", input.DetailedIOConditions.Count > 0 ? string.Join(", ", input.DetailedIOConditions) : "" },
                { "Device", input.Device ?? "" },
                { "DeviceNumber", input.DeviceNumber.ToString() },
                { "InterlockNumber", input.InterlockNumber.ToString() }
            };

            // IO情報プレースホルダーを追加
            AddIoPlaceholders(replacements, input.IoInfoList);

            return replacements;
        }

        /// <summary>
        /// IO情報プレースホルダーを辞書に追加
        /// 最初のIOの情報（IO.xxx形式）とインデックス付きIO情報（IO[n].xxx形式）の両方を追加
        /// </summary>
        /// <param name="replacements">プレースホルダー辞書</param>
        /// <param name="ioInfoList">IO情報リスト</param>
        private static void AddIoPlaceholders(Dictionary<string, string?> replacements, List<InterlockIoInfo> ioInfoList)
        {
            // 最初のIOの情報を追加（後方互換性のため）
            var firstIo = ioInfoList.FirstOrDefault();
            if (firstIo != null)
            {
                replacements.Add("IO.Address", firstIo.Address);
                replacements.Add("IO.IOName", firstIo.IOName ?? "");
                replacements.Add("IO.IOExplanation", firstIo.IOExplanation ?? "");
                replacements.Add("IO.XComment", firstIo.XComment ?? "");
                replacements.Add("IO.YComment", firstIo.YComment ?? "");
                replacements.Add("IO.FComment", firstIo.FComment ?? "");
                replacements.Add("IO.IOSpot", firstIo.IOSpot ?? "");
                replacements.Add("IO.UnitName", firstIo.UnitName ?? "");
                replacements.Add("IO.System", firstIo.System ?? "");
                replacements.Add("IO.StationNumber", firstIo.StationNumber ?? "");
                replacements.Add("IO.LinkDevice", firstIo.LinkDevice ?? "");
                replacements.Add("IO.IsOnCondition", firstIo.IsOnCondition ? "ON" : "OFF");
            }
            else
            {
                // IOがない場合は空文字で置換
                AddEmptyIoPlaceholder(replacements, "IO");
            }

            // インデックス付きIO情報プレースホルダーを追加 (例: {IO[0].Address}, {IO[1].IOName})
            for (int i = 0; i < ioInfoList.Count; i++)
            {
                var io = ioInfoList[i];
                var prefix = $"IO[{i}]";
                replacements.Add($"{prefix}.Address", io.Address);
                replacements.Add($"{prefix}.IOName", io.IOName ?? "");
                replacements.Add($"{prefix}.IOExplanation", io.IOExplanation ?? "");
                replacements.Add($"{prefix}.XComment", io.XComment ?? "");
                replacements.Add($"{prefix}.YComment", io.YComment ?? "");
                replacements.Add($"{prefix}.FComment", io.FComment ?? "");
                replacements.Add($"{prefix}.IOSpot", io.IOSpot ?? "");
                replacements.Add($"{prefix}.UnitName", io.UnitName ?? "");
                replacements.Add($"{prefix}.System", io.System ?? "");
                replacements.Add($"{prefix}.StationNumber", io.StationNumber ?? "");
                replacements.Add($"{prefix}.LinkDevice", io.LinkDevice ?? "");
                replacements.Add($"{prefix}.IsOnCondition", io.IsOnCondition ? "ON" : "OFF");
            }
        }

        /// <summary>
        /// 空のIO情報プレースホルダーを追加
        /// </summary>
        /// <param name="replacements">プレースホルダー辞書</param>
        /// <param name="prefix">プレースホルダーのプレフィックス（"IO" または "IO[n]"）</param>
        private static void AddEmptyIoPlaceholder(Dictionary<string, string?> replacements, string prefix)
        {
            replacements.Add($"{prefix}.Address", "");
            replacements.Add($"{prefix}.IOName", "");
            replacements.Add($"{prefix}.IOExplanation", "");
            replacements.Add($"{prefix}.XComment", "");
            replacements.Add($"{prefix}.YComment", "");
            replacements.Add($"{prefix}.FComment", "");
            replacements.Add($"{prefix}.IOSpot", "");
            replacements.Add($"{prefix}.UnitName", "");
            replacements.Add($"{prefix}.System", "");
            replacements.Add($"{prefix}.StationNumber", "");
            replacements.Add($"{prefix}.LinkDevice", "");
            replacements.Add($"{prefix}.IsOnCondition", "");
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
                { "ConIO", input.ConIO?.DisplayCondition ?? "" }
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
        /// プレースホルダーを置換
        /// UI表示用の装飾記号（●○）や空白文字を削除してエラーメッセージを整形します
        /// これらの記号はUIでの選択状態表示に使われるため、エラーメッセージには不要です
        /// </summary>
        /// <param name="template">テンプレート文字列（例: "{CylinderName}のインターロック異常"）</param>
        /// <param name="values">プレースホルダー値のディクショナリ</param>
        /// <returns>置換後の文字列</returns>
        public string ReplacePlaceholders(string? template, Dictionary<string, string?> values)
        {
            if (string.IsNullOrEmpty(template))
            {
                return string.Empty;
            }

            var result = template;
            foreach (var kvp in values)
            {
                if (kvp.Value == null)
                {
                    continue;
                }

                // UI表示用の装飾記号と空白を削除してエラーメッセージを整形
                var value = kvp.Value?.Replace(FILLED_CIRCLE, "")
                                       .Replace(HOLLOW_CIRCLE, "")
                                       .Replace(HALF_WIDTH_SPACE, "")
                                       .Replace(FULL_WIDTH_SPACE, "");
                result = result.Replace($"{{{kvp.Key}}}", value ?? "");
            }
            return result;
        }

        /// <summary>
        /// Row_1からシリンダー名とGoOrBack表示名を解析
        /// 例: "CY01All" → ("CY01", "All")
        /// </summary>
        private static (string cylinderName, string goOrBack) ParseRow1(string? row1)
        {
            if (string.IsNullOrEmpty(row1))
            {
                return ("", "");
            }

            // 末尾の "All", "Go", "Back" を検出
            if (row1.EndsWith("All"))
            {
                return (row1[..^3], "All");
            }
            if (row1.EndsWith("Go"))
            {
                return (row1[..^2], "Go");
            }
            if (row1.EndsWith("Back"))
            {
                return (row1[..^4], "Back");
            }

            return (row1, "");
        }

        /// <summary>
        /// GoOrBack表示名を数値に変換
        /// </summary>
        private static int ParseGoOrBack(string? display)
        {
            return display switch
            {
                "All" or "Go&Back" => 0,
                "Go" or "GoOnly" => 1,
                "Back" or "BackOnly" => 2,
                _ => 0
            };
        }

        /// <summary>
        /// カテゴリIDに応じたAlarmIdリストを取得
        /// (既存のErrorService.SaveMnemonicDeviceOperationのロジックを踏襲)
        /// </summary>
        private static List<int> GetAlarmIdsForCategory(int? categoryId)
        {
            return categoryId switch
            {
                2 or 29 or 30 => [1, 2, 5], // 保持
                3 or 9 or 15 or 27 => [1, 2, 3, 4, 5], // 速度制御INV1
                4 or 10 or 16 or 28 => [1, 2, 3, 4, 3, 4, 5], // 速度制御INV2
                5 or 11 or 17 => [1, 2, 3, 4, 3, 4, 3, 4, 5], // 速度制御INV3
                6 or 12 or 18 => [1, 2, 3, 4, 3, 4, 3, 4, 3, 4, 5], // 速度制御INV4
                7 or 13 or 19 => [1, 2, 3, 4, 3, 4, 3, 4, 3, 4, 3, 4, 5], // 速度制御INV5
                20 => [5], // バネ
                31 => [], // サーボ
                _ => []
            };
        }

        /// <summary>
        /// デフォルトのInterlockエラーメッセージを取得
        /// </summary>
        private static List<ErrorMessage> GetDefaultInterlockErrorMessages()
        {
            return
            [
                new ErrorMessage
                {
                    MnemonicId = (int)MnemonicType.Interlock,
                    ConditionTypeId = 0, // 共通テンプレート
                    AlarmId = 1,
                    BaseMessage = "{CylinderName}のインターロック異常",
                    BaseAlarm = "ｲﾝﾀﾛｯｸ異常",
                    Category1 = "ｲﾝﾀﾛｯｸｴﾗｰ",
                    Category2 = "{GoBack}",
                    Category3 = "{ConditionType}",
                    DefaultCountTime = 1000
                }
            ];
        }

        /// <summary>
        /// ConditionTypeIdに基づいてメッセージテンプレートを選択
        /// 1. 指定されたConditionTypeIdに一致するメッセージを優先
        /// 2. 見つからない場合はConditionTypeId=0（共通）のメッセージを使用
        /// 3. それも見つからない場合はデフォルトメッセージを使用
        /// </summary>
        private static List<ErrorMessage> GetMessagesForConditionType(
            List<ErrorMessage> allMessages,
            int? conditionTypeId)
        {
            // 指定されたConditionTypeIdに一致するメッセージを検索
            var targetConditionTypeId = conditionTypeId ?? 0;
            var specificMessages = allMessages
                .Where(m => m.ConditionTypeId == targetConditionTypeId)
                .ToList();

            if (specificMessages.Count > 0)
            {
                return specificMessages;
            }

            // ConditionTypeId=0の汎用メッセージを検索
            var generalMessages = allMessages
                .Where(m => m.ConditionTypeId == 0)
                .ToList();

            if (generalMessages.Count > 0)
            {
                return generalMessages;
            }

            // デフォルトメッセージを返す
            return GetDefaultInterlockErrorMessages();
        }

        /// <summary>
        /// CylinderInterlockDataからInterlock入力データを生成（詳細情報付き）
        /// </summary>
        /// <param name="cylinderInterlockDataList">デバイス割り当て済みのCylinderInterlockDataリスト</param>
        /// <param name="plcId">PLC ID</param>
        /// <returns>InterlockErrorInputリスト（詳細情報付き）</returns>
        public List<InterlockErrorInput> BuildInterlockErrorInputsFromCylinderData(
            List<CylinderInterlockData> cylinderInterlockDataList,
            int plcId)
        {
            var inputs = new List<InterlockErrorInput>();

            foreach (var cylinderData in cylinderInterlockDataList)
            {
                var cylinder = cylinderData.Cylinder;
                var cylinderName = $"{cylinder.CYNum}{cylinder.CYNameSub}";

                foreach (var interlockData in cylinderData.Interlocks)
                {
                    var interlock = interlockData.Interlock;
                    var goOrBackDisplay = interlockData.GoOrBackDisplayName;

                    // 条件シリンダー名を構築
                    var conditionCylinderName = interlockData.ConditionCylinder != null
                        ? $"{interlockData.ConditionCylinder.CYNum}{interlockData.ConditionCylinder.CYNameSub}"
                        : "";

                    // 前提条件情報を構築
                    var precondition1Info = BuildPrecondition1Info(interlockData.Precondition1);
                    var precondition2Info = BuildPrecondition2Info(interlockData.Precondition2);

                    foreach (var conditionData in interlockData.Conditions)
                    {
                        var condition = conditionData.Condition;
                        var conditionTypeName = conditionData.ConditionType?.ConditionTypeName ?? $"条件{condition.ConditionNumber}";

                        // IO条件のリストを構築（DisplayConditionプロパティを使用）
                        var ioConditions = conditionData.IOs
                            .Select(io => io.DisplayCondition)
                            .ToList();

                        // 詳細IO条件のリストを構築（説明付き）
                        var detailedIOConditions = conditionData.IOs
                            .Select(io => io.DetailedDisplayCondition)
                            .ToList();

                        // IO詳細情報を構築（IoIndexでソートしてインデックスを付与）
                        var ioInfoList = conditionData.IOs
                            .OrderBy(io => io.IO.IoIndex)
                            .Select((io, idx) => new InterlockIoInfo
                            {
                                Index = idx,
                                Address = io.IOAddress,
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
                                IsOnCondition = io.IO.IsOnCondition
                            })
                            .ToList();

                        var input = new InterlockErrorInput
                        {
                            CylinderId = cylinder.Id,
                            CylinderName = cylinderName,
                            InterlockSortId = interlock.SortId,
                            GoOrBack = interlock.GoOrBack,
                            GoOrBackDisplayName = goOrBackDisplay,
                            ConditionCylinderId = condition.CylinderId,
                            ConditionCylinderName = conditionCylinderName,
                            ConditionNumber = condition.ConditionNumber,
                            ConditionTypeId = condition.ConditionTypeId,
                            ConditionTypeName = conditionTypeName,
                            ConditionName = condition.Name,
                            ConditionDevice = condition.Device,
                            ConditionComment1 = condition.Comment1,
                            ConditionComment2 = condition.Comment2,
                            Device = conditionData.Device,
                            DeviceNumber = conditionData.DeviceNumber,
                            InterlockNumber = conditionData.InterlockNumber,
                            PlcId = plcId,
                            Precondition1Info = precondition1Info,
                            Precondition2Info = precondition2Info,
                            IOConditions = ioConditions,
                            DetailedIOConditions = detailedIOConditions,
                            // 複合キーのため、識別子として組み合わせを使用
                            InterlockId = CalculateInterlockId(interlock.CylinderId, interlock.SortId),
                            InterlockConditionId = CalculateInterlockConditionId(condition.CylinderId, condition.InterlockSortId, condition.ConditionNumber),
                            IoInfoList = ioInfoList
                        };

                        inputs.Add(input);
                    }
                }
            }

            return inputs;
        }

        /// <summary>
        /// 前提条件1の情報文字列を構築
        /// </summary>
        private static string? BuildPrecondition1Info(InterlockPrecondition1? precondition1)
        {
            if (precondition1 == null)
            {
                return null;
            }

            // Precondition1の情報を組み合わせ
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(precondition1.ConditionName))
            {
                parts.Add(precondition1.ConditionName);
            }

            if (!string.IsNullOrEmpty(precondition1.Description))
            {
                parts.Add(precondition1.Description);
            }

            return parts.Count > 0 ? string.Join(" ", parts) : null;
        }

        /// <summary>
        /// 前提条件2の情報文字列を構築
        /// </summary>
        private static string? BuildPrecondition2Info(InterlockPrecondition2? precondition2)
        {
            if (precondition2 == null)
            {
                return null;
            }

            var parts = new List<string>();

            // InterlockModeがある場合
            if (!string.IsNullOrEmpty(precondition2.InterlockMode))
            {
                parts.Add($"Mode:{precondition2.InterlockMode}");
            }

            // 工程範囲がある場合
            if (precondition2.StartDetailId.HasValue && precondition2.StartDetailId > 0)
            {
                parts.Add($"開始工程:{precondition2.StartDetailId}");
            }

            if (precondition2.EndDetailId.HasValue && precondition2.EndDetailId > 0)
            {
                parts.Add($"終了工程:{precondition2.EndDetailId}");
            }

            return parts.Count > 0 ? string.Join(" ", parts) : null;
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
            List<Operation> operations,
            List<IO> ioList,
            int plcId,
            int cycleId)
        {
            var inputs = new List<OperationErrorInput>();

            foreach (var operation in operations)
            {
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

                // 速度センサーのIOを取得（CategoryIdに基づく）
                var speedIOs = GetSpeedSensorIOs(ioList, operation);

                var input = new OperationErrorInput
                {
                    OperationId = operation.Id,
                    OperationName = operation.OperationName,
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
        /// CategoryIdに基づいて速度センサーのIOを取得
        /// </summary>
        /// <param name="ioList">IOリスト</param>
        /// <param name="operation">Operation情報</param>
        /// <returns>速度センサーのIOリスト</returns>
        private static List<OperationIoInfo> GetSpeedSensorIOs(List<IO> ioList, Operation operation)
        {
            var speedIOs = new List<OperationIoInfo>();

            // CategoryIdに応じて速度センサーを取得
            switch (operation.CategoryId)
            {
                case 3 or 9 or 15 or 27: // 速度変化1回
                    if (!string.IsNullOrEmpty(operation.SS1))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS1));
                    }

                    break;

                case 4 or 10 or 16 or 28: // 速度変化2回
                    if (!string.IsNullOrEmpty(operation.SS1))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS1));
                    }

                    if (!string.IsNullOrEmpty(operation.SS2))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS2));
                    }

                    break;

                case 5 or 11 or 17: // 速度変化3回
                    if (!string.IsNullOrEmpty(operation.SS1))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS1));
                    }

                    if (!string.IsNullOrEmpty(operation.SS2))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS2));
                    }

                    if (!string.IsNullOrEmpty(operation.SS3))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS3));
                    }

                    break;

                case 6 or 12 or 18: // 速度変化4回
                    if (!string.IsNullOrEmpty(operation.SS1))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS1));
                    }

                    if (!string.IsNullOrEmpty(operation.SS2))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS2));
                    }

                    if (!string.IsNullOrEmpty(operation.SS3))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS3));
                    }

                    if (!string.IsNullOrEmpty(operation.SS4))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS4));
                    }

                    break;

                case 7 or 13 or 19: // 速度変化5回
                    if (!string.IsNullOrEmpty(operation.SS1))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS1));
                    }

                    if (!string.IsNullOrEmpty(operation.SS2))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS2));
                    }

                    if (!string.IsNullOrEmpty(operation.SS3))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS3));
                    }

                    if (!string.IsNullOrEmpty(operation.SS4))
                    {
                        speedIOs.AddRange(FindIOs(ioList, operation.SS4));
                    }
                    // Note: 5回目の速度センサーは未定義のため4つまで
                    break;

                default:
                    // その他のカテゴリは速度センサーなし
                    break;
            }

            return speedIOs;
        }

        /// <summary>
        /// CategoryIdからカテゴリ名を取得
        /// </summary>
        private static string? GetCategoryName(int? categoryId)
        {
            return categoryId switch
            {
                2 or 29 or 30 => "保持",
                3 or 9 or 15 or 27 => "速度制御INV1",
                4 or 10 or 16 or 28 => "速度制御INV2",
                5 or 11 or 17 => "速度制御INV3",
                6 or 12 or 18 => "速度制御INV4",
                7 or 13 or 19 => "速度制御INV5",
                20 => "バネ",
                31 => "サーボ",
                _ => $"Category{categoryId}"
            };
        }

        /// <summary>
        /// InterlockIdを計算
        /// 複合キー（CylinderId, SortId）から一意のIDを生成
        /// </summary>
        /// <param name="cylinderId">シリンダーID</param>
        /// <param name="sortId">ソートID</param>
        /// <returns>計算されたInterlockId</returns>
        private static int CalculateInterlockId(int cylinderId, int sortId)
        {
            return cylinderId * INTERLOCK_ID_MULTIPLIER + sortId;
        }

        /// <summary>
        /// InterlockConditionIdを計算
        /// 複合キー（CylinderId, InterlockSortId, ConditionNumber）から一意のIDを生成
        /// </summary>
        /// <param name="cylinderId">シリンダーID</param>
        /// <param name="interlockSortId">InterlockソートID</param>
        /// <param name="conditionNumber">条件番号</param>
        /// <returns>計算されたInterlockConditionId</returns>
        private static int CalculateInterlockConditionId(int cylinderId, int interlockSortId, int conditionNumber)
        {
            return cylinderId * INTERLOCK_CONDITION_ID_BASE_MULTIPLIER
                   + interlockSortId * INTERLOCK_CONDITION_ID_SORT_MULTIPLIER
                   + conditionNumber;
        }
    }
}

