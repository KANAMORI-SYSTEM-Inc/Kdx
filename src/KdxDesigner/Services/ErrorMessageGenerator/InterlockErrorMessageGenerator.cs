using Kdx.Contracts.DTOs;
using Kdx.Contracts.Enums;
using Kdx.Infrastructure.Supabase.Repositories;
using KdxDesigner.Models;

namespace KdxDesigner.Services.ErrorMessageGenerator
{
    /// <summary>
    /// Interlock用エラーメッセージ生成サービス
    /// メモリストアのデータを元にGeneratedErrorを生成する
    /// </summary>
    public class InterlockErrorMessageGenerator : ErrorMessageGeneratorBase, IInterlockErrorMessageGenerator
    {
        // InterlockエラーInputのID計算用定数
        private const int INTERLOCK_ID_MULTIPLIER = 1000;
        private const int INTERLOCK_CONDITION_ID_BASE_MULTIPLIER = 100000;
        private const int INTERLOCK_CONDITION_ID_SORT_MULTIPLIER = 100;

        public InterlockErrorMessageGenerator(ISupabaseRepository repository) : base(repository)
        {
        }

        /// <summary>
        /// Interlock用エラーメッセージを生成
        /// ProcessErrorを作成・保存し、それを元にGeneratedErrorを生成する
        /// </summary>
        public async Task<List<GeneratedError>> GenerateInterlockErrorsAsync(
            List<InterlockErrorInput> inputs,
            int startErrorNum,
            int deviceStartM,
            int deviceStartT)
        {
            var errors = new List<GeneratedError>();
            var processErrors = new List<ProcessError>();

            // 全てのInterlock用メッセージを取得（ConditionTypeIdでグループ化するため）
            var allMessages = await _repository.GetAllErrorMessagesAsync();
            var interlockMessages = allMessages
                .Where(m => m.MnemonicId == (int)MnemonicType.Interlock)
                .ToList();

            int errorNum = startErrorNum;
            int alarmCount = 0;

            foreach (var input in inputs)
            {
                // プレースホルダー置換用辞書を構築
                var replacements = BuildInterlockReplacements(input);

                // IO情報を収集
                var (ioAddresses, ioNames, ioConditions) = CollectInterlockIoInfo(input);

                // ConditionTypeIdに基づいてメッセージを選択
                var messages = GetMessagesForConditionType(interlockMessages, input.ConditionTypeId);

                foreach (var msg in messages)
                {
                    // ProcessErrorを作成（中間データとして保存）
                    var processError = new ProcessError
                    {
                        PlcId = input.PlcId,
                        CycleId = null,
                        Device = input.Device,
                        MnemonicId = (int)MnemonicType.Interlock,
                        RecordId = input.InterlockConditionId,
                        AlarmId = input.DeviceNumber,
                        AlarmCount = alarmCount++,
                        ErrorNum = input.DeviceNumber,
                        Comment1 = ReplacePlaceholders(msg.Category1, replacements),
                        Comment2 = ReplacePlaceholders(msg.Category2, replacements),
                        Comment3 = ReplacePlaceholders(msg.Category3, replacements),
                        Comment4 = "",
                        AlarmComment = ReplacePlaceholders(msg.BaseAlarm, replacements),
                        MessageComment = ReplacePlaceholders(msg.BaseMessage, replacements),
                        ErrorTime = 0,  // インターロックはタイマーが無い、即時エラー発生
                        ErrorTimeDevice = string.Empty,
                        IoAddresses = ioAddresses,
                        IoNames = ioNames,
                        IoConditions = ioConditions
                    };

                    processErrors.Add(processError);

                    // ProcessErrorからGeneratedErrorを生成
                    var error = new GeneratedError
                    {
                        PlcId = processError.PlcId ?? 0,
                        ErrorNum = processError.ErrorNum ?? 0,
                        MnemonicId = processError.MnemonicId ?? 0,
                        AlarmId = processError.AlarmId ?? 0,
                        RecordId = processError.RecordId,
                        DeviceM = processError.Device,
                        DeviceT = string.Empty, // インターロックはTデバイスを使用しない
                        Comment1 = processError.Comment1,
                        Comment2 = processError.Comment2,
                        Comment3 = processError.Comment3,
                        Comment4 = processError.Comment4,
                        AlarmComment = processError.AlarmComment,
                        MessageComment = processError.MessageComment,
                        ErrorTime = processError.ErrorTime ?? 0,
                        CycleId = processError.CycleId
                    };

                    errors.Add(error);
                    errorNum++;
                }
            }

            // ProcessErrorをデータベースに保存
            if (processErrors.Count > 0)
            {
                await _repository.SaveErrorsAsync(processErrors);
            }

            return errors;
        }

        /// <summary>
        /// Interlock用のIO情報を収集
        /// </summary>
        /// <param name="input">Interlockエラー入力データ</param>
        /// <returns>(IoAddresses, IoNames, IoConditions)のタプル</returns>
        private static (string?, string?, string?) CollectInterlockIoInfo(InterlockErrorInput input)
        {
            var addresses = new List<string>();
            var names = new List<string>();
            var conditions = new List<string>();

            foreach (var io in input.IoInfoList)
            {
                addresses.Add(io.Address);
                if (!string.IsNullOrEmpty(io.IOName))
                {
                    names.Add(io.IOName);
                    var condition = io.IsOnCondition ? "ON" : "OFF";
                    conditions.Add($"{io.IOName}:{condition}");
                }
            }

            var ioAddresses = addresses.Any() ? string.Join(",", addresses) : null;
            var ioNames = names.Any() ? string.Join(",", names) : null;
            var ioConditions = conditions.Any() ? string.Join(", ", conditions) : null;

            return (ioAddresses, ioNames, ioConditions);
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
