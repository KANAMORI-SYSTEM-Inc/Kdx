using Kdx.Contracts.DTOs;
using Kdx.Contracts.DTOs.MnemonicCommon;
using Kdx.Contracts.Interfaces;
using System.Threading;

namespace Kdx.Core.Application
{
    /// <summary>
    /// Handles the output of interlock mnemonics.
    /// </summary>
    public sealed class InterlockMnemonicOutput : IInterlockMnemonicOutput
    {
        private readonly IInterlockMnemonicStrategyResolver _strategyResolver;

        public InterlockMnemonicOutput(IInterlockMnemonicStrategyResolver strategyResolver)
        {
            _strategyResolver = strategyResolver ?? throw new ArgumentNullException(nameof(strategyResolver));
        }

        public List<LadderCsvRow> On_1(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("ON_1", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle,  mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> On_2(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("ON_2", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> On_Or(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("ON_OR", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> On_M(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("ON_M", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> Off_1(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("OFF_1", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> Limit(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("OFF_1", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> Dev(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("DEV", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> Range(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("RANGE", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> Srv(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("SRV", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> Thr(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("ThR", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> Inv_Al(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("INV_AL", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> Inv_M(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("INV_M", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> IL(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("IL", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        public List<LadderCsvRow> ANY(
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
            List<InterlockIO> interlockIOs)
        {
            return BuildInternal("ANY", plcId, ErrorNumber, ErrorDevice, ErrorOutputDevice, cycle, mnemonicDevices, cylinder,
                interlock, precondition1, precondition2, conditions, interlockIOs);
        }

        private List<LadderCsvRow> BuildInternal(
            string key,
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
            List<InterlockIO> interlockIOs)
        {
            ErrorOutputDevice = ErrorOutputDevice ?? string.Empty;
            conditions = conditions ?? new List<InterlockCondition>();
            interlockIOs = interlockIOs ?? new List<InterlockIO>();

            var strategy = _strategyResolver.Resolve(key);
            List<LadderCsvRow> rows = new List<LadderCsvRow>();

            // コンテキストの生成
            var context = new InterlockMnemonicContext(
                ErrorNumber,
                ErrorDevice,
                ErrorOutputDevice,
                plcId,
                cycle,
                mnemonicDevices.AsReadOnly(),
                cylinder,
                interlock,
                precondition1,
                precondition2,
                conditions.AsReadOnly(),
                interlockIOs.AsReadOnly()
            );

            // 基本のInterlock条件(センサ関連)
            rows.AddRange(strategy.Build(context));

            // 1つ目の前提条件
            rows.AddRange(BuildPrecondition1(
                plcId,
                ErrorNumber,
                ErrorDevice,
                ErrorOutputDevice,
                cycle,
                cylinder,
                interlock,
                precondition1,
                precondition2,
                conditions,
                interlockIOs));

            // 2つ目の前提条件
            rows.AddRange(BuildPrecondition2(
                plcId,
                ErrorNumber,
                ErrorDevice,
                ErrorOutputDevice,
                cycle,
                mnemonicDevices,
                cylinder,
                interlock,
                precondition1,
                precondition2,
                conditions,
                interlockIOs));

            // エラー出力とエラーナンバー設定
            rows.Add(LadderRow.AddOUT(ErrorOutputDevice));
            rows.AddRange(LadderRow.AddMOVSet("K" + ErrorNumber.ToString(), ErrorDevice));

            return rows;
        }

        private List<LadderCsvRow> BuildPrecondition1(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1? precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs)
        {
            ErrorOutputDevice = ErrorOutputDevice ?? string.Empty;
            conditions = conditions ?? new List<InterlockCondition>();
            interlockIOs = interlockIOs ?? new List<InterlockIO>();
            var strategy = _strategyResolver.Resolve("ON_1");
            List<LadderCsvRow> rows = new List<LadderCsvRow>();

            // 前提条件1の内容に応じて回路を追加
            if (precondition1 != null)
            {
                switch (precondition1.Id)
                {
                    case 1:
                        rows.Add(LadderRow.AddAND(cycle.StartDevice));
                        break;
                    case 2:
                        rows.Add(LadderRow.AddAND(cycle.PauseDevice));
                        break;
                    default:
                        break;
                }
            }

            return rows;
        }

        private List<LadderCsvRow> BuildPrecondition2(
            int plcId,
            int ErrorNumber,
            string ErrorDevice,
            string ErrorOutputDevice,
            Cycle cycle,
            List<MnemonicDeviceWithProcessDetail> mnemonicDevices,
            Cylinder cylinder,
            Interlock interlock,
            InterlockPrecondition1? precondition1,
            InterlockPrecondition2? precondition2,
            List<InterlockCondition> conditions,
            List<InterlockIO> interlockIOs)
        {
            ErrorOutputDevice = ErrorOutputDevice ?? string.Empty;
            conditions = conditions ?? new List<InterlockCondition>();
            interlockIOs = interlockIOs ?? new List<InterlockIO>();
            var strategy = _strategyResolver.Resolve("ON_1");
            List<LadderCsvRow> rows = new List<LadderCsvRow>();

            if (precondition2 != null)
            {
                var startDetail = mnemonicDevices.FirstOrDefault(d => d.Detail.Id == precondition2.StartDetailId);
                var endDetail = mnemonicDevices.FirstOrDefault(d => d.Detail.Id == precondition2.EndDetailId);

                if (startDetail != null)
                {
                    rows.Add(LadderRow.AddAND(startDetail.Mnemonic.DeviceLabel + (startDetail.Mnemonic.StartNum + 0).ToString()));

                    if (endDetail != null)
                    {
                        rows.Add(LadderRow.AddAND(endDetail.Mnemonic.DeviceLabel + (endDetail.Mnemonic.StartNum + 4).ToString()));
                    }
                }
            }
            return rows;
        }
    }
}
