using Kdx.Contracts.DTOs;
using Kdx.Contracts.Interfaces;
using KdxDesigner.Services.MnemonicDevice;
using KdxDesigner.ViewModels;
using KdxDesigner.Models;

namespace KdxDesigner.Utils.Interlock
{
    /// <summary>
    /// インターロックラダー生成のメインビルダー
    /// CylinderBuilderと同様の構造で、InterlockLadderGeneratorを使用してラダーを生成する
    /// </summary>
    public class InterlockBuilder
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IErrorAggregator _errorAggregator;
        private readonly IIOAddressService _ioAddressService;
        private readonly IMnemonicDeviceMemoryStore? _memoryStore;

        public InterlockBuilder(
            MainViewModel mainViewModel,
            IErrorAggregator errorAggregator,
            IIOAddressService ioAddressService)
        {
            _mainViewModel = mainViewModel;
            _errorAggregator = errorAggregator;
            _ioAddressService = ioAddressService;
            _memoryStore = mainViewModel._mnemonicMemoryStore;
        }

        /// <summary>
        /// インターロックラダーを生成
        /// </summary>
        /// <param name="cylinders">シリンダーデバイスリスト</param>
        /// <param name="ioList">IOリスト</param>
        /// <param name="plcId">PLC ID</param>
        /// <param name="processDetails">ProcessDetailとMnemonicDeviceの結合リスト</param>
        /// <returns>生成されたラダー行のリスト</returns>
        public Task<List<LadderCsvRow>> GenerateLadder(
            List<MnemonicDeviceWithCylinder> cylinders,
            List<IO> ioList,
            int plcId,
            List<MnemonicDeviceWithProcessDetail> processDetails)
        {
            LadderCsvRow.ResetKeyCounter();
            var result = new List<LadderCsvRow>();

            // メモリストアからCylinderInterlockDataを取得（メモリ保存時に構築済み）
            var allInterlockDataList = _memoryStore?.GetCylinderInterlockData(plcId)
                ?? new List<CylinderInterlockData>();

            if (allInterlockDataList.Count == 0)
            {
                _errorAggregator.AddError(new OutputError
                {
                    Message = "インターロックデータがありません。メモリ保存を先に実行してください。",
                    IsCritical = true
                });
                return Task.FromResult(result);
            }

            // InterlockLadderGeneratorを使用してラダーを生成
            var ladderGenerator = new InterlockLadderGenerator(_mainViewModel, _errorAggregator, _ioAddressService);

            foreach (var cylinderInterlockData in allInterlockDataList)
            {
                // 対応するMnemonicDeviceWithCylinderを検索
                var cylinderDevice = cylinders.FirstOrDefault(c => c.Cylinder.Id == cylinderInterlockData.Cylinder.Id);

                if (cylinderDevice == null)
                {
                    _errorAggregator.AddError(new OutputError
                    {
                        RecordId = cylinderInterlockData.Cylinder.Id,
                        RecordName = cylinderInterlockData.Cylinder.CYNum,
                        Message = $"シリンダー {cylinderInterlockData.Cylinder.CYNum} のデバイス情報が見つかりません。",
                        IsCritical = false
                    });
                    continue;
                }

                // InterlockConditionTypeに応じたラダー生成
                var ladderRows = ladderGenerator.GenerateLadder(
                    ioList,
                    cylinderInterlockData,
                    cylinderDevice,
                    processDetails);
                result.AddRange(ladderRows);

                // 前進/後退インターロック出力を生成
                var goBackOutput = ladderGenerator.GenerateGoBackInterlockOutput(
                    cylinderInterlockData,
                    cylinderDevice);
                result.AddRange(goBackOutput);
            }

            return Task.FromResult(result);
        }
    }
}
