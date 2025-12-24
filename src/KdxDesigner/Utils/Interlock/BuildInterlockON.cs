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
    internal class BuildInterlockON
    {
        private readonly IErrorAggregator _errorAggregator;

        public BuildInterlockON(IErrorAggregator errorAggregator)
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
            List<InterlockIOData> ios)
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

            // IOsをIoIndexでソート（順序を保証）
            var sortedIOs = ios.OrderBy(io => io.IoIndex).ToList();

            string device = conditionData.Device;
            string conditionTypeName = conditionType?.ConditionTypeName ?? "不明";
            string comment1 = condition.Comment1 ?? "";
            string comment2 = condition.Comment2 ?? "";

            // 条件のコメント
            // result.Add(LadderRow.AddStatement($"    Cond:{conditionData.InterlockNumber} {conditionTypeName} {comment1} {comment2}"));

            switch (condition.ConditionTypeId)
            {
                case 1: // ON_1
                    result.AddRange(GenerateON1(ios));
                    break;
                case 2: // ON_2
                    result.AddRange(GenerateON2(ios));
                    break;
                case 3: // ON_OR
                    result.AddRange(GenerateONOR(ios));
                    break;
                case 4: // ON_M
                    result.AddRange(GenerateONM(ios));
                    break;
                case 5: // OFF_1
                case 6: // LIMIT
                    result.AddRange(GenerateOFF1(ios));
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

        public List<LadderCsvRow> GenerateON1(
            List<InterlockIOData> ios)
        {
            List<LadderCsvRow> result = new();

            var index0 = ios.FirstOrDefault(io => io.IoIndex == 0);
            if (index0 != null)
            {
                result.Add(LadderRow.AddANI(index0.IO.IOAddress));
            }
            return result;
        }

        public List<LadderCsvRow> GenerateON2(
            List<InterlockIOData> ios)
        {
            List<LadderCsvRow> result = new();

            var index0 = ios.FirstOrDefault(io => io.IoIndex == 0);
            var index1 = ios.FirstOrDefault(io => io.IoIndex == 1);
            if (index0 != null && index1 != null)
            {
                result.Add(LadderRow.AddLD(index0.IO.IOAddress));
                result.Add(LadderRow.AddORI(index1.IO.IOAddress));
                result.Add(LadderRow.AddANB());
            }
            return result;
        }

        public List<LadderCsvRow> GenerateONOR(
            List<InterlockIOData> ios)
        {
            List<LadderCsvRow> result = new();

            var index0 = ios.FirstOrDefault(io => io.IoIndex == 0);
            var index1 = ios.FirstOrDefault(io => io.IoIndex == 1);
            if (index0 != null && index1 != null)
            {
                result.Add(LadderRow.AddLD(index0.IO.IOAddress));
                result.Add(LadderRow.AddAND(index1.IO.IOAddress));

                result.Add(LadderRow.AddLDI(index0.IO.IOAddress));
                result.Add(LadderRow.AddANI(index1.IO.IOAddress));

                result.Add(LadderRow.AddORB());
                result.Add(LadderRow.AddANB());
            }
            return result;
        }

        public List<LadderCsvRow> GenerateONM(
            List<InterlockIOData> ios)
        {
            List<LadderCsvRow> result = new();

            var isFirst = true;
            var isSecond = false;
            foreach (var io in ios)
            {
                if (isFirst)
                {
                    result.Add(LadderRow.AddLD(io.IO.IOAddress));
                    isFirst = false;
                }
                else
                {
                    result.Add(LadderRow.AddORI(io.IO.IOAddress));
                    isSecond = true;
                }
            }

            if (!isSecond)
            {
                var resultOnlyFirst = new List<LadderCsvRow>();
                resultOnlyFirst.Add(LadderRow.AddAND(ios[0].IO.IOAddress));
                return resultOnlyFirst;
            }
            else
            {
                result.Add(LadderRow.AddORB());
            }

            return result;
        }

        public List<LadderCsvRow> GenerateOFF1(
            List<InterlockIOData> ios)
        {
            List<LadderCsvRow> result = new();

            var index0 = ios.FirstOrDefault(io => io.IoIndex == 0);
            if (index0 != null)
            {
                result.Add(LadderRow.AddAND(index0.IO.IOAddress));
            }
            return result;
        }
    }
}
