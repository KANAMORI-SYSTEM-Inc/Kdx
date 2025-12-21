
using Kdx.Contracts.DTOs;
using Kdx.Contracts.DTOs.MnemonicCommon;
using Kdx.Contracts.Enums;
using Kdx.Contracts.Interfaces;
using KdxDesigner.Models;

namespace KdxDesigner.Utils.Interlock
{
    /// <summary>
    /// IO条件によるインターロックラダー生成モジュール
    /// </summary>
    internal class BuildPreCondition
    {
        private readonly IErrorAggregator _errorAggregator;

        public BuildPreCondition(IErrorAggregator errorAggregator)
        {

            _errorAggregator = errorAggregator;
        }

        /// <summary>
        /// InterlockPrecondition1からラダーを生成
        /// </summary>
        /// <param name="interlockData">親のインターロックデータ</param>
        /// <param name="label">デバイスラベル</param>
        /// <param name="startNum">開始番号</param>
        /// <returns>生成されたラダー行のリスト</returns>
        public List<LadderCsvRow> PreCondition1(
            InterlockData interlockData, string label, int startNum)
        {
            var result = new List<LadderCsvRow>();



            // デバイスが割り当てられていない場合はエラー
            if (interlockData.Precondition1 == null)
            {
                _errorAggregator.AddError(new OutputError
                {
                    MnemonicId = (int)MnemonicType.Interlock,
                    RecordId = interlockData.Interlock.CylinderId,
                    RecordName = $"IL:{interlockData.Interlock.CylinderId}-{interlockData.Interlock.SortId}/Cond:{interlockData.Interlock.SortId}",
                    Message = $"インターロック前提条件1が割り当てられていません。"
                });
                return result;
            }
            string forwardManualDevice = $"{label}{startNum + 10}"; // 例: X1001
            string backwardManualDevice = $"{label}{startNum + 11}"; // 例: X1002
            string forwardAutoDevice = $"{label}{startNum + 12}"; // 例: X1001
            string backwardAutoDevice = $"{label}{startNum + 13}"; // 例: X1002

            switch (interlockData.Precondition1.Id)
            {
                case 1:
                    // 自動運転
                    if (interlockData.Interlock.GoOrBack == 0)  // どちらも
                    {
                        // どちらもの場合の追加条件
                        result.Add(LadderRow.AddLD(forwardAutoDevice));
                        result.Add(LadderRow.AddOR(backwardAutoDevice));

                    }
                    else if (interlockData.Interlock.GoOrBack == 1) // 進む
                    {
                        // 進む場合の追加条件
                        result.Add(LadderRow.AddLD(forwardAutoDevice));

                    }
                    else if (interlockData.Interlock.GoOrBack == 2) // 戻る
                    {
                        // 戻る場合の追加条件
                        result.Add(LadderRow.AddLD(backwardAutoDevice));

                    }
                    result.Add(LadderRow.AddAND(SettingsManager.Settings.PauseSignal));
                    break;
                case 2:
                    // 各個操作
                    if (interlockData.Interlock.GoOrBack == 0)  // どちらも
                    {
                        // どちらもの場合の追加条件
                        result.Add(LadderRow.AddLD(forwardManualDevice));
                        result.Add(LadderRow.AddOR(backwardManualDevice));

                    }
                    else if (interlockData.Interlock.GoOrBack == 1) // 進む
                    {
                        // 進む場合の追加条件
                        result.Add(LadderRow.AddLD(forwardManualDevice));

                    }
                    else if (interlockData.Interlock.GoOrBack == 2) // 戻る
                    {
                        // 戻る場合の追加条件
                        result.Add(LadderRow.AddLD(backwardManualDevice));

                    }
                    result.Add(LadderRow.AddANI(SettingsManager.Settings.PauseSignal));
                    break;
                case 3:
                    // 常時
                    // 各個操作
                    if (interlockData.Interlock.GoOrBack == 0)  // どちらも
                    {
                        // どちらもの場合の追加条件
                        result.Add(LadderRow.AddLD(forwardManualDevice));
                        result.Add(LadderRow.AddOR(backwardManualDevice));
                        result.Add(LadderRow.AddOR(forwardAutoDevice));
                        result.Add(LadderRow.AddOR(backwardAutoDevice));
                    }
                    else if (interlockData.Interlock.GoOrBack == 1) // 進む
                    {
                        // 進む場合の追加条件
                        result.Add(LadderRow.AddLD(forwardManualDevice));
                        result.Add(LadderRow.AddOR(forwardAutoDevice));
                    }
                    else if (interlockData.Interlock.GoOrBack == 2) // 戻る
                    {
                        // 戻る場合の追加条件
                        result.Add(LadderRow.AddLD(backwardManualDevice));
                        result.Add(LadderRow.AddOR(backwardAutoDevice));
                    }
                    result.Add(LadderRow.AddAND(SettingsManager.Settings.AlwaysON));
                    break;
                default:
                    _errorAggregator.AddError(new OutputError
                    {
                        MnemonicId = (int)MnemonicType.Interlock,
                        RecordId = interlockData.Interlock.CylinderId,
                        RecordName = $"IL:{interlockData.Interlock.CylinderId}-{interlockData.Interlock.SortId}/PreCond1",
                        Message = $"インターロック前提条件1の型が不明です。"
                    });
                    break;
            }
            return result;
        }

        /// <summary>
        /// InterlockPrecondition2からラダーを生成
        /// </summary>
        /// <param name="interlockData">親のインターロックデータ</param>
        /// <param name="processDetails">ProcessDetailとMnemonicDeviceの結合リスト</param>
        /// <returns>生成されたラダー行のリスト</returns>
        public List<LadderCsvRow> PreCondition2(
            InterlockData interlockData,
            List<MnemonicDeviceWithProcessDetail> processDetails)
        {
            var result = new List<LadderCsvRow>();

            var preCondition2 = interlockData.Precondition2;
            if (preCondition2 == null)
            {
                return result;
            }

            // StartDetailIdとEndDetailIdの範囲内のProcessDetailを取得
            var startDetailId = preCondition2.StartDetailId;
            var endDetailId = preCondition2.EndDetailId;

            // 範囲内のProcessDetailをフィルタリング
            var filteredDetails = processDetails
                .Where(pd => pd.Detail.Id >= startDetailId && pd.Detail.Id <= endDetailId)
                .OrderBy(pd => pd.Detail.Id)
                .ToList();

            var startDatail = processDetails.FirstOrDefault(pd => pd.Detail.Id == startDetailId);
            var endDatail = processDetails.FirstOrDefault(pd => pd.Detail.Id == endDetailId);

            // TODO: Precondition2のロジックに基づいてラダーを生成

            return result;
        }

        /// <summary>
        /// InterlockPrecondition3からラダーを生成
        /// </summary>
        /// <param name="interlockData">親のインターロックデータ</param>
        /// <returns>生成されたラダー行のリスト</returns>
        public List<LadderCsvRow> PreCondition3(
            InterlockData interlockData)
        {
            var result = new List<LadderCsvRow>();
            var preCondition3 = interlockData.Precondition3;
            if (preCondition3 == null)
            {
                return result;
            }

            // アドレスを取得
            var address = preCondition3.ConditionType switch
            {
                "IO" => preCondition3.IOAddress,
                "Device" => preCondition3.DeviceAddress,
                _ => null
            };

            // アドレスがnullまたは空の場合はエラー
            if (string.IsNullOrEmpty(address))
            {
                _errorAggregator.AddError(new OutputError
                {
                    MnemonicId = (int)MnemonicType.Interlock,
                    RecordId = interlockData.Interlock.CylinderId,
                    RecordName = $"IL:{interlockData.Interlock.CylinderId}-{interlockData.Interlock.SortId}/PreCond3",
                    Message = $"インターロック前提条件3のアドレスが設定されていません。(ConditionType={preCondition3.ConditionType})"
                });
                return result;
            }

            // ON条件かOFF条件かでAND/ANIを切り替え
            if (preCondition3.IsOnCondition)
            {
                result.Add(LadderRow.AddAND(address));
            }
            else
            {
                result.Add(LadderRow.AddANI(address));
            }

            return result;
        }
    }
}