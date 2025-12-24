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
    internal class BuildInterlockINV
    {
        private readonly IErrorAggregator _errorAggregator;
        private readonly IIOAddressService _ioAddressService;


        public BuildInterlockINV(IErrorAggregator errorAggregator, IIOAddressService ioAddressService)
        {
            _errorAggregator = errorAggregator;
            _ioAddressService = ioAddressService;
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
            List<IO> ioList,
            MnemonicDeviceWithCylinder cylinderDevice)
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
            string conditionTypeName = conditionType?.ConditionTypeName ?? "不明";
            string comment1 = condition.Comment1 ?? "";
            string comment2 = condition.Comment2 ?? "";

            // 条件のコメント
            // result.Add(LadderRow.AddStatement($"    Cond:{conditionData.InterlockNumber} {conditionTypeName} {comment1} {comment2}"));

            switch (condition.ConditionTypeId)
            {
                case 12: // INV_AL
                    result.AddRange(GenerateInvAl(cylinderDevice, ioList));
                    break;
                case 13: // INV_M
                    result.AddRange(GenerateInvM(cylinderDevice, ioList));
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

        public List<LadderCsvRow> GenerateInvAl(
            MnemonicDeviceWithCylinder cylinderDevice,
            List<IO> ioList)
        {
            List<LadderCsvRow> result = new();

            string sensorKey = cylinderDevice.Cylinder.CYNum + "AL";
            string? sensorAddress = _ioAddressService.GetSingleAddress(
                                        ioList,
                                        sensorKey,
                                        false,
                                        "IL_INV_AL",
                                        (int)MnemonicType.Interlock,
                                        null);

            if (sensorAddress == null)
            {
                _errorAggregator.AddError(new OutputError
                {
                    MnemonicId = (int)MnemonicType.Interlock,
                    RecordId = cylinderDevice.Cylinder.Id,
                    RecordName = cylinderDevice.Cylinder.CYNum,
                    Message = $"INV_AL用センサーIO '{sensorKey}' が見つかりません。"
                });
                return result;
            }
            result.Add(LadderRow.AddANI(sensorAddress!));

            return result;
        }

        public List<LadderCsvRow> GenerateInvM(
            MnemonicDeviceWithCylinder cylinderDevice,
            List<IO> ioList)
        {
            List<LadderCsvRow> result = new();

            string sensorKey = "CM-" + cylinderDevice.Cylinder.CYNum + "M";
            string? sensorAddress = _ioAddressService.GetSingleAddress(
                                        ioList,
                                        sensorKey,
                                        true,
                                        "IL_INV_M",
                                        (int)MnemonicType.Interlock,
                                        null);

            if (sensorAddress == null)
            {
                _errorAggregator.AddError(new OutputError
                {
                    MnemonicId = (int)MnemonicType.Interlock,
                    RecordId = cylinderDevice.Cylinder.Id,
                    RecordName = cylinderDevice.Cylinder.CYNum,
                    Message = $"INV_M用センサーIO '{sensorKey}' が見つかりません。"
                });
                return result;
            }
            result.Add(LadderRow.AddANI(sensorAddress!));

            return result;
        }
    }
}
