using Kdx.Contracts.DTOs;

namespace Kdx.Contracts.Interfaces
{
    /// <summary>
    /// ProsTime（工程時間）デバイスの管理サービスインターフェース
    /// </summary>
    public interface IProsTimeDeviceService
    {
        /// <summary>
        /// ProsTimeテーブルの全レコードを削除する
        /// </summary>
        void DeleteProsTimeTable();

        /// <summary>
        /// PLCIDに基づいてProsTimeを取得する
        /// </summary>
        List<ProsTime> GetProsTimeByPlcId(int plcId);

        /// <summary>
        /// MnemonicIdに基づいてProsTimeを取得する
        /// </summary>
        List<ProsTime> GetProsTimeByMnemonicId(int plcId, int mnemonicId);

        /// <summary>
        /// Operationに基づいてProsTimeを保存する
        /// </summary>
        void SaveProsTime(List<Operation> operations, int startCurrent, int startPrevious, int startCylinder, int plcId);
    }
}