using Kdx.Contracts.DTOs;
using KdxDesigner.Models;

namespace KdxDesigner.Services.ErrorMessageGenerator
{
    /// <summary>
    /// Interlock用エラーメッセージ生成サービスのインターフェース
    /// </summary>
    public interface IInterlockErrorMessageGenerator
    {
        /// <summary>
        /// Interlock用エラーメッセージを生成
        /// </summary>
        /// <param name="inputs">Interlock入力データリスト</param>
        /// <param name="startErrorNum">開始エラー番号</param>
        /// <param name="deviceStartM">Mデバイス開始番号</param>
        /// <param name="deviceStartT">Tデバイス開始番号</param>
        /// <returns>生成されたGeneratedErrorリスト</returns>
        Task<List<GeneratedError>> GenerateInterlockErrorsAsync(
            List<InterlockErrorInput> inputs,
            int startErrorNum,
            int deviceStartM,
            int deviceStartT);

        /// <summary>
        /// CylinderInterlockDataからInterlock入力データを生成（詳細情報付き）
        /// </summary>
        /// <param name="cylinderInterlockDataList">デバイス割り当て済みのCylinderInterlockDataリスト</param>
        /// <param name="plcId">PLC ID</param>
        /// <returns>InterlockErrorInputリスト（詳細情報付き）</returns>
        List<InterlockErrorInput> BuildInterlockErrorInputsFromCylinderData(
            List<CylinderInterlockData> cylinderInterlockDataList,
            int plcId);
    }
}
