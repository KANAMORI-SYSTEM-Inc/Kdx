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
    internal class BuildInterlockOutput
    {
        private readonly IErrorAggregator _errorAggregator;

        public BuildInterlockOutput(IErrorAggregator errorAggregator)
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
        public List<LadderCsvRow> Generate(InterlockConditionData conditionData)
        {
            var result = new List<LadderCsvRow>();

            // IOsをIoIndexでソート（順序を保証）
            string outcoil = "M" + conditionData.DeviceNumber;

            result.Add(LadderRow.AddOUT(outcoil));
            result.AddRange(LadderRow.AddMOVSet(
                $"K{conditionData.InterlockNumber}",
                SettingsManager.Settings.ErrorDevice));

            return result;
        }
    }
}
