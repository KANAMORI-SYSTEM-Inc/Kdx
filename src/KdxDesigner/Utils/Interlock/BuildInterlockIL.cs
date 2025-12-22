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
    /// <param name="ioAddressService">IO検索用のサービス</param>
    ///
    internal class BuildInterlockIL
    {
        private readonly IErrorAggregator _errorAggregator;

        public BuildInterlockIL(IErrorAggregator errorAggregator, IIOAddressService ioAddressService)
        {
            _errorAggregator = errorAggregator;
        }

        /// <summary>
        /// InterlockConditionDataからIO条件のラダーを生成
        /// </summary>
        /// <param name="conditionData">インターロック条件データ</param>
        /// <param name="interlockData">親のインターロックデータ</param>
        /// <param name="ios">InterlockIOデータのリスト</param>
        /// <returns>生成されたラダー行のリスト</returns>
        public List<LadderCsvRow> Generate(
            InterlockConditionData conditionData,
            InterlockData interlockData,
            List<InterlockIO> ios)
        {
            var result = new List<LadderCsvRow>();
            var condition = conditionData.Condition;
            var conditionType = conditionData.ConditionType;

            // デバイスが割り当てられていない場合はエラー
            if (string.IsNullOrEmpty(conditionData.Device))
            {
                _errorAggregator.AddError(new OutputError
                {
                    MnemonicId = (int)MnemonicType.Interlock,
                    RecordId = condition.CylinderId,
                    RecordName = $"IL:{interlockData.Interlock.CylinderId}-{interlockData.Interlock.SortId}/Cond:{condition.ConditionNumber}",
                    Message = $"インターロック条件のデバイスが割り当てられていません。メモリ保存を実行してください。"
                });
                return result;
            }

            string device = conditionData.Device;
            bool isOnCondition = conditionData.Condition.IsOnCondition ?? true;
            string conditionTypeName = conditionType?.ConditionTypeName ?? "不明";
            string comment1 = condition.Comment1 ?? "";
            string comment2 = condition.Comment2 ?? "";



            // 条件のコメント
            result.Add(LadderRow.AddStatement($"    Cond:{conditionData.InterlockNumber} {conditionTypeName} {comment1} {comment2}"));

            switch (condition.ConditionTypeId)
            {
                case 14: // IL
                case 15: // ANY
                    result.AddRange(GenerateInterlock(device, isOnCondition));
                    break;
                case 16: // IL_IO
                    result.AddRange(GenerateInterlockIO(ios, isOnCondition));
                    break;
                default:
                    _errorAggregator.AddError(new OutputError
                    {
                        MnemonicId = (int)MnemonicType.Interlock,
                        RecordId = condition.CylinderId,
                        RecordName = $"IL:{interlockData.Interlock.CylinderId}-{interlockData.Interlock.SortId}/Cond:{condition.ConditionNumber}",
                        Message = $"不明なインターロック条件タイプID: {condition.ConditionTypeId}"
                    });
                    break;
            }

            return result;
        }

        public List<LadderCsvRow> GenerateInterlock(
            string device,
            bool isOnCondition)
        {
            List<LadderCsvRow> result = new();

            if (isOnCondition)
            {
                result.Add(LadderRow.AddAND(device));
            }
            else
            {
                result.Add(LadderRow.AddANI(device));
            }

            return result;
        }

        public List<LadderCsvRow> GenerateInterlockIO(
            List<InterlockIO> ios,
            bool isOnCondition)
        {
            List<LadderCsvRow> result = new();

            string address = ios.First().IOAddress;

            result.Add(LadderRow.AddANI(address!));

            return result;
        }
    }
}
