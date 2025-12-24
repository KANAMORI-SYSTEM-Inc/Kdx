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
                // プレースホルダー置換用辞書を構築（拡張版）
                var replacements = new Dictionary<string, string?>
                {
                    { "CylinderName", input.CylinderName },
                    { "GoBack", input.GoOrBackDisplayName },
                    { "ConditionCylinderName", input.ConditionCylinderName },
                    { "ConditionType", input.ConditionTypeName },
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

                // IO情報用プレースホルダーを追加（最初のIOの情報を使用 - 後方互換性）
                var firstIo = input.IoInfoList.FirstOrDefault();
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
                    replacements.Add("IO.Address", "");
                    replacements.Add("IO.IOName", "");
                    replacements.Add("IO.IOExplanation", "");
                    replacements.Add("IO.XComment", "");
                    replacements.Add("IO.YComment", "");
                    replacements.Add("IO.FComment", "");
                    replacements.Add("IO.IOSpot", "");
                    replacements.Add("IO.UnitName", "");
                    replacements.Add("IO.System", "");
                    replacements.Add("IO.StationNumber", "");
                    replacements.Add("IO.LinkDevice", "");
                    replacements.Add("IO.IsOnCondition", "");
                }

                // インデックス付きIO情報プレースホルダーを追加 (例: {IO[0].Address}, {IO[1].IOName})
                for (int i = 0; i < input.IoInfoList.Count; i++)
                {
                    var io = input.IoInfoList[i];
                    replacements.Add($"IO[{i}].Address", io.Address);
                    replacements.Add($"IO[{i}].IOName", io.IOName ?? "");
                    replacements.Add($"IO[{i}].IOExplanation", io.IOExplanation ?? "");
                    replacements.Add($"IO[{i}].XComment", io.XComment ?? "");
                    replacements.Add($"IO[{i}].YComment", io.YComment ?? "");
                    replacements.Add($"IO[{i}].FComment", io.FComment ?? "");
                    replacements.Add($"IO[{i}].IOSpot", io.IOSpot ?? "");
                    replacements.Add($"IO[{i}].UnitName", io.UnitName ?? "");
                    replacements.Add($"IO[{i}].System", io.System ?? "");
                    replacements.Add($"IO[{i}].StationNumber", io.StationNumber ?? "");
                    replacements.Add($"IO[{i}].LinkDevice", io.LinkDevice ?? "");
                    replacements.Add($"IO[{i}].IsOnCondition", io.IsOnCondition ? "ON" : "OFF");
                }

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
                var replacements = new Dictionary<string, string?>
                {
                    { "OperationName", input.OperationName },
                    { "Valve1", input.Valve1 },
                    { "Valve2", input.Valve2 },
                    { "GoBack", input.GoBack },
                    { "CategoryName", input.CategoryName }
                };

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
                            InterlockId = interlock.CylinderId * 1000 + interlock.SortId,
                            InterlockConditionId = condition.CylinderId * 100000 + condition.InterlockSortId * 100 + condition.ConditionNumber,
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
    }
}

