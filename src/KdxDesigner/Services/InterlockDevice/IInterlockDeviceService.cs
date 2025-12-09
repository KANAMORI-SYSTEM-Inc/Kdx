using KdxDesigner.Models;

namespace KdxDesigner.Services.InterlockDevice
{
    /// <summary>
    /// InterlockConditionにMデバイスを割り当てるサービスのインターフェース
    /// </summary>
    public interface IInterlockDeviceService
    {
        /// <summary>
        /// CylinderInterlockDataリストの全InterlockConditionにデバイスを割り当てます
        /// </summary>
        /// <param name="cylinderInterlockDataList">デバイス割り当て対象のデータリスト</param>
        /// <param name="deviceStartM">Mデバイスの開始番号</param>
        /// <param name="startNum">インターロック開始番号</param>
        /// <returns>割り当てられたデバイス数</returns>
        int AssignDevices(
            List<CylinderInterlockData> cylinderInterlockDataList,
            int deviceStartM,
            int startNum);

        /// <summary>
        /// 割り当て結果からMemoryエンティティのリストを生成します
        /// </summary>
        /// <param name="cylinderInterlockDataList">デバイス割り当て済みのデータリスト</param>
        /// <param name="plcId">PLC ID</param>
        /// <returns>Memoryエンティティのリスト</returns>
        List<Kdx.Contracts.DTOs.Memory> GenerateMemoryEntities(
            List<CylinderInterlockData> cylinderInterlockDataList,
            int plcId);
    }
}
