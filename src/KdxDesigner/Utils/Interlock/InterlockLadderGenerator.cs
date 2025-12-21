using Kdx.Contracts.DTOs;
using Kdx.Contracts.DTOs.MnemonicCommon;
using Kdx.Contracts.Enums;
using Kdx.Contracts.Interfaces;
using KdxDesigner.Models;
using KdxDesigner.ViewModels;

namespace KdxDesigner.Utils.Interlock
{
    /// <summary>
    /// インターロック用のラダープログラムを生成するクラス
    /// CylinderBuilderと同様の構造で、InterlockConditionTypeに応じて
    /// 異なるモジュールを使い分ける
    /// </summary>
    public class InterlockLadderGenerator
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IErrorAggregator _errorAggregator;

        // 各InterlockConditionType用のビルダー
        private readonly BuildInterlockON _onBuilder;

        public InterlockLadderGenerator(
            MainViewModel mainViewModel,
            IErrorAggregator errorAggregator)
        {
            _mainViewModel = mainViewModel;
            _errorAggregator = errorAggregator;

            // モジュールの初期化
            _onBuilder = new BuildInterlockON(errorAggregator);
        }

        /// <summary>
        /// CylinderInterlockDataからラダープログラムを生成
        /// </summary>
        /// <param name="cylinderData">シリンダーのインターロックデータ</param>
        /// <param name="cylinder">シリンダーデバイス情報</param>
        /// <param name="ioList">IOリスト</param>
        /// <param name="processDetails">ProcessDetailとMnemonicDeviceの結合リスト</param>
        /// <returns>生成されたラダー行のリスト</returns>
        public List<LadderCsvRow> GenerateLadder(
            CylinderInterlockData cylinderData,
            MnemonicDeviceWithCylinder cylinder,
            List<MnemonicDeviceWithProcessDetail> processDetails)
        {
            var result = new List<LadderCsvRow>();

            if (cylinderData.Interlocks.Count == 0)
            {
                return result;
            }

            // シリンダー情報
            string cyNum = cylinderData.Cylinder.CYNum ?? "";
            string cyNumSub = cylinderData.Cylinder.CYNameSub ?? "";
            string cyName = cyNum + cyNumSub;

            // 行間ステートメント
            result.Add(LadderRow.AddStatement($"{cyName} インターロック"));

            foreach (var interlockData in cylinderData.Interlocks)
            {
                result.AddRange(GenerateInterlockLadder(
                    interlockData,
                    cylinder,
                    processDetails));
            }

            result.Add(LadderRow.AddNOP());

            return result;
        }

        /// <summary>
        /// 個別のInterlockDataからラダーを生成
        /// </summary>
        private List<LadderCsvRow> GenerateInterlockLadder(
            InterlockData interlockData,
            MnemonicDeviceWithCylinder cylinderDevice,
            List<MnemonicDeviceWithProcessDetail> processDetails)
        {
            var result = new List<LadderCsvRow>();
            var interlock = interlockData.Interlock;
            string goOrBackText = interlockData.GoOrBackDisplayName;

            // Interlockのコメント
            result.Add(LadderRow.AddStatement($"  IL:{interlock.CylinderId}-{interlock.SortId} {goOrBackText}"));

            var buildPrecondition = new BuildPreCondition(_errorAggregator);
            var buildOutput = new BuildInterlockOutput(_errorAggregator);

            var label = cylinderDevice.Mnemonic.DeviceLabel; // ラベルの取得  
            var startNum = cylinderDevice.Mnemonic.StartNum; // ラベルの取得  

            foreach (var conditionData in interlockData.Conditions)
            {
                // InterlockConditionTypeに応じてモジュールを使い分け
                var conditionTypeId = conditionData.ConditionType?.Id ?? 0;

                // ConditionDataに紐づくIOリストを取得
                var ios = conditionData.IOs;

                result.AddRange(buildPrecondition.PreCondition1(interlockData, label, startNum));
                result.AddRange(buildPrecondition.PreCondition2(interlockData, processDetails));
                result.AddRange(buildPrecondition.PreCondition3(interlockData));

                switch (conditionTypeId)
                {
                    case 1: // ON_1
                    case 2: // ON_2
                    case 3: // ON_OR_1
                    case 4: // ON_M
                    case 5: // OFF_1
                    case 6: // LIMIT
                        result.AddRange(_onBuilder.Generate(conditionData, interlockData, ios));
                        result.AddRange(buildOutput.Generate(conditionData));
                        break;
                    case 7:     // DEV
                    case 8:     // RANGE
                    case 9:     // 未定義
                    case 10:    // SRV
                    case 11:    // ThR
                        _errorAggregator.AddError(new OutputError
                        {
                            MnemonicId = (int)MnemonicType.Interlock,
                            RecordId = conditionData.Condition.CylinderId,
                            RecordName = $"IL:{interlock.CylinderId}-{interlock.SortId}/Cond:{conditionData.Condition.ConditionNumber}",
                            Message = $"インターロック条件タイプ '{conditionData.ConditionType?.ConditionTypeName}' は未対応です。"
                        });
                        break;

                    case 12: // INV_AL
                    case 13: // INV_M
                        result.AddRange(_onBuilder.Generate(conditionData, interlockData, ios));
                        result.AddRange(buildOutput.Generate(conditionData));
                        break;
                    case 14: // IL
                    case 15: // ANY
                    case 16: // IL_IO
                        result.AddRange(_onBuilder.Generate(conditionData, interlockData, ios));
                        result.AddRange(buildOutput.Generate(conditionData));
                        break;
                    default:
                        _errorAggregator.AddError(new OutputError
                        {
                            MnemonicId = (int)MnemonicType.Interlock,
                            RecordId = conditionData.Condition.CylinderId,
                            RecordName = $"IL:{interlock.CylinderId}-{interlock.SortId}/Cond:{conditionData.Condition.ConditionNumber}",
                            Message = $"不明なインターロック条件タイプID '{conditionTypeId}' です。"
                        });
                        break;
                }
            }
            return result;
        }

        /// <summary>
        /// 前進/後退インターロック出力を生成
        /// </summary>
        public List<LadderCsvRow> GenerateGoBackInterlockOutput(
            CylinderInterlockData cylinderData,
            MnemonicDeviceWithCylinder cylinder)
        {
            var result = new List<LadderCsvRow>();
            var cylinderLabel = cylinder.Mnemonic.DeviceLabel;
            var cylinderStartNum = cylinder.Mnemonic.StartNum;

            // Go方向（前進）のインターロック出力
            var goInterlocks = cylinderData.Interlocks
                .Where(i => i.Interlock.GoOrBack == 0 || i.Interlock.GoOrBack == 1) // Go&Back or GoOnly
                .SelectMany(i => i.Conditions)
                .ToList();

            if (goInterlocks.Count > 0)
            {
                result.Add(LadderRow.AddStatement($"  前進インターロック出力"));
                result.AddRange(GenerateInterlockORCondition(goInterlocks, cylinderLabel, cylinderStartNum, "Go"));
            }

            // Back方向（後退）のインターロック出力
            var backInterlocks = cylinderData.Interlocks
                .Where(i => i.Interlock.GoOrBack == 0 || i.Interlock.GoOrBack == 2) // Go&Back or BackOnly
                .SelectMany(i => i.Conditions)
                .ToList();

            if (backInterlocks.Count > 0)
            {
                result.Add(LadderRow.AddStatement($"  後退インターロック出力"));
                result.AddRange(GenerateInterlockORCondition(backInterlocks, cylinderLabel, cylinderStartNum, "Back"));
            }

            return result;
        }

        /// <summary>
        /// インターロック条件のOR出力を生成
        /// </summary>
        private List<LadderCsvRow> GenerateInterlockORCondition(
            List<InterlockConditionData> conditions,
            string? cylinderLabel,
            int cylinderStartNum,
            string direction)
        {
            var result = new List<LadderCsvRow>();

            if (conditions.Count == 0)
            {
                return result;
            }

            // インターロック出力先（例: M30000 + offset）
            // 実際のオフセットは設計に応じて調整
            int outputOffset = direction == "Go" ? 50 : 51;
            string outputDevice = $"{cylinderLabel}{cylinderStartNum + outputOffset}";

            // 最初の条件はLD
            var firstCondition = conditions[0];
            if (!string.IsNullOrEmpty(firstCondition.Device))
            {
                result.Add(LadderRow.AddLD(firstCondition.Device));

                // 2番目以降の条件はOR
                for (int i = 1; i < conditions.Count; i++)
                {
                    var condition = conditions[i];
                    if (!string.IsNullOrEmpty(condition.Device))
                    {
                        result.Add(LadderRow.AddOR(condition.Device));
                    }
                }

                result.Add(LadderRow.AddOUT(outputDevice));
            }

            return result;
        }
    }
}
