using Kdx.Contracts.DTOs;
using KdxDesigner.Models;

namespace KdxDesigner.Services.ErrorMessageGenerator
{
    /// <summary>
    /// エラーメッセージ生成サービスのインターフェース
    /// メモリストアのデータを元にGeneratedErrorを生成する
    /// </summary>
    public interface IErrorMessageGenerator
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
        /// Operation用エラーメッセージを生成
        /// </summary>
        /// <param name="inputs">Operation入力データリスト</param>
        /// <param name="startErrorNum">開始エラー番号</param>
        /// <param name="deviceStartM">Mデバイス開始番号</param>
        /// <param name="deviceStartT">Tデバイス開始番号</param>
        /// <returns>生成されたGeneratedErrorリスト</returns>
        Task<List<GeneratedError>> GenerateOperationErrorsAsync(
            List<OperationErrorInput> inputs,
            int startErrorNum,
            int deviceStartM,
            int deviceStartT);

        /// <summary>
        /// プレースホルダーを置換
        /// </summary>
        /// <param name="template">テンプレート文字列</param>
        /// <param name="values">プレースホルダー値のディクショナリ</param>
        /// <returns>置換後の文字列</returns>
        string ReplacePlaceholders(string? template, Dictionary<string, string?> values);

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
