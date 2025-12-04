using Kdx.Contracts.DTOs;
using Kdx.Contracts.DTOs.MnemonicCommon;

namespace Kdx.Contracts.Interfaces
{
    /// <summary>
    /// Interlockニモニック出力のインターフェース
    /// 既存の呼び出し側との互換性を維持
    /// </summary>
    public interface IInterlockMnemonicOutput
    {
        /// <summary>
        /// ON_1 パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> On_1(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        /// <summary>
        /// ON_2 パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> On_2(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        // 将来的にOn_3, On_4...を追加する場合はここに追加

        /// <summary>
        /// ON_Or パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> On_Or(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        /// <summary>
        /// ON_M パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> On_M(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        /// <summary>
        /// OFF_1 パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> Off_1(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        /// <summary>
        /// OFF_1 パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> Limit(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        /// <summary>
        /// DEV パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> Dev(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        /// <summary>
        /// Range パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> Range(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        /// <summary>
        /// SRV パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> Srv(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        /// <summary>
        /// ThR パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> Thr(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        /// <summary>
        /// INV_AL パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> Inv_Al(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        /// <summary>
        /// INV_M パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> Inv_M(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        /// <summary>
        /// IL パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> IL(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);

        /// <summary>
        /// IL パターンでInterlock回路を生成
        /// </summary>
        List<LadderCsvRow> ANY(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1 precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs);
    }
}
