using Kdx.Contracts.DTOs;
using KdxDesigner.Models;

namespace KdxDesigner.Services.ErrorMessageGenerator
{
    /// <summary>
    /// Operation用エラーメッセージ生成サービスのインターフェース
    /// </summary>
    public interface IOperationErrorMessageGenerator
    {
        /// <summary>
        /// Operation用エラーメッセージを生成
        /// 1. OperationとCategoryIdからProcessErrorを生成
        /// 2. ProcessErrorとOperationからOperationErrorInputを構築
        /// 3. OperationErrorInputからGeneratedErrorを生成
        /// </summary>
        /// <param name="operations">Operationリスト</param>
        /// <param name="ioList">IOリスト</param>
        /// <param name="plcId">PLC ID</param>
        /// <param name="cycleId">Cycle ID</param>
        /// <param name="startErrorNum">開始エラー番号</param>
        /// <param name="deviceStartM">Mデバイス開始番号</param>
        /// <param name="deviceStartT">Tデバイス開始番号</param>
        /// <returns>生成されたGeneratedErrorリスト</returns>
        Task<List<GeneratedError>> GenerateOperationErrorsAsync(
            List<Operation> operations,
            List<IO> ioList,
            int plcId,
            int cycleId,
            int startErrorNum,
            int deviceStartM,
            int deviceStartT);

        /// <summary>
        /// OperationリストからOperation入力データを生成
        /// </summary>
        /// <param name="operations">Operationリスト</param>
        /// <param name="ioList">IOリスト</param>
        /// <param name="plcId">PLC ID</param>
        /// <param name="cycleId">Cycle ID</param>
        /// <returns>OperationErrorInputリスト</returns>
        List<OperationErrorInput> BuildOperationErrorInputsFromOperations(
            List<ProcessError> errors,
            List<Operation> operations,
            List<IO> ioList,
            int plcId,
            int cycleId);
    }
}
