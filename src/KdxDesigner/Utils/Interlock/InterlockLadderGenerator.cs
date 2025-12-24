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
        private readonly IIOAddressService _ioAddressService;

        // 各InterlockConditionType用のビルダー
        private readonly BuildInterlockON _onBuilder;
        private readonly BuildInterlockINV _invBuilder;
        private readonly BuildInterlockIL _ilBuilder;
        private List<string> _outcoilDevices;

        public InterlockLadderGenerator(
            MainViewModel mainViewModel,
            IErrorAggregator errorAggregator,
            IIOAddressService ioAddressService)
        {
            _mainViewModel = mainViewModel;
            _errorAggregator = errorAggregator;
            _ioAddressService = ioAddressService;

            // モジュールの初期化
            _onBuilder = new BuildInterlockON(errorAggregator);
            _invBuilder = new BuildInterlockINV(errorAggregator, ioAddressService);
            _ilBuilder = new BuildInterlockIL(errorAggregator, ioAddressService);
            _outcoilDevices = new List<string>();
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
            List<IO> ioList,
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

            _outcoilDevices.Clear();

            // 行間ステートメント
            result.Add(LadderRow.AddStatement($"{cyName} インターロック"));
            foreach (var interlockData in cylinderData.Interlocks)
            {
                result.AddRange(GenerateInterlockLadder(
                    ioList,
                    interlockData,
                    cylinder,
                    processDetails));

            }

            var Label = cylinder.Mnemonic.DeviceLabel; // ラベルの取得
            var StartNum = cylinder.Mnemonic.StartNum; // ラベルの取得

            result.AddRange(GenerateInterlockLadder_Outcoil(
                Label,
                StartNum));

            result.Add(LadderRow.AddNOP());

            return result;
        }

        /// <summary>
        /// 個別のInterlockDataからラダーを生成
        /// </summary>
        private List<LadderCsvRow> GenerateInterlockLadder(
            List<IO> ioList,
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
                        _outcoilDevices.Add("M" + conditionData.DeviceNumber);
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
                        result.AddRange(_invBuilder.Generate(conditionData, interlockData, ioList, cylinderDevice));
                        result.AddRange(buildOutput.Generate(conditionData));
                        _outcoilDevices.Add("M" + conditionData.DeviceNumber);
                        break;
                    case 14: // IL
                    case 15: // ANY
                    case 16: // IL_IO
                        result.AddRange(_ilBuilder.Generate(conditionData, interlockData, ios));
                        result.AddRange(buildOutput.Generate(conditionData));
                        _outcoilDevices.Add("M" + conditionData.DeviceNumber);
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

        private List<LadderCsvRow> GenerateInterlockLadder_Outcoil(
            string Label,
            int StartNum)
        {
            var result = new List<LadderCsvRow>();

            // 行き手動
            result.Add(LadderRow.AddLD($"{Label}{StartNum + 10}"));
            foreach (var outcoil in _outcoilDevices)
            {
                if (outcoil == null)
                {
                    continue;
                }

                result.Add(LadderRow.AddANI(outcoil));
            }
            result.Add(LadderRow.AddOUT($"{Label}{StartNum + 17}"));

            // 行き自動
            result.Add(LadderRow.AddLD($"{Label}{StartNum + 12}"));
            foreach (var outcoil in _outcoilDevices)
            {
                if (outcoil == null)
                {
                    continue;
                }

                result.Add(LadderRow.AddANI(outcoil));
            }
            result.Add(LadderRow.AddOUT($"{Label}{StartNum + 15}"));

            // 帰り手動
            result.Add(LadderRow.AddLD($"{Label}{StartNum + 11}"));
            foreach (var outcoil in _outcoilDevices)
            {
                if (outcoil == null)
                {
                    continue;
                }

                result.Add(LadderRow.AddANI(outcoil));
            }
            result.Add(LadderRow.AddOUT($"{Label}{StartNum + 18}"));

            // 帰り自動
            result.Add(LadderRow.AddLD($"{Label}{StartNum + 13}"));
            foreach (var outcoil in _outcoilDevices)
            {
                if (outcoil == null)
                {
                    continue;
                }

                result.Add(LadderRow.AddANI(outcoil));
            }
            result.Add(LadderRow.AddOUT($"{Label}{StartNum + 16}"));
            return result;
        }
    }
}