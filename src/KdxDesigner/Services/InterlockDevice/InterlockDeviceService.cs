using Kdx.Contracts.DTOs;
using Kdx.Contracts.Enums;
using KdxDesigner.Models;

namespace KdxDesigner.Services.InterlockDevice
{
    /// <summary>
    /// InterlockConditionにMデバイスを割り当てるサービス
    /// </summary>
    public class InterlockDeviceService : IInterlockDeviceService
    {
        /// <summary>
        /// CylinderInterlockDataリストの全InterlockConditionにデバイスを割り当てます
        /// </summary>
        /// <param name="cylinderInterlockDataList">デバイス割り当て対象のデータリスト</param>
        /// <param name="deviceStartM">Mデバイスの開始番号</param>
        /// <param name="startNum">インターロック開始番号</param>
        /// <returns>割り当てられたデバイス数</returns>
        public int AssignDevices(
            List<CylinderInterlockData> cylinderInterlockDataList,
            int deviceStartM,
            int startNum)
        {
            int currentNum = startNum;

            foreach (var cylinderData in cylinderInterlockDataList)
            {
                foreach (var interlockData in cylinderData.Interlocks)
                {
                    foreach (var conditionData in interlockData.Conditions)
                    {
                        // インターロック番号を割り当て
                        conditionData.InterlockNumber = currentNum;

                        // デバイス番号を計算
                        conditionData.DeviceNumber = deviceStartM + currentNum;

                        // Mデバイス文字列を生成
                        conditionData.Device = $"M{conditionData.DeviceNumber}";

                        currentNum++;
                    }
                }
            }

            // 割り当てられたデバイス数を返す
            return currentNum - startNum;
        }

        /// <summary>
        /// 割り当て結果からMemoryエンティティのリストを生成します
        /// </summary>
        /// <param name="cylinderInterlockDataList">デバイス割り当て済みのデータリスト</param>
        /// <param name="plcId">PLC ID</param>
        /// <returns>Memoryエンティティのリスト</returns>
        public List<Memory> GenerateMemoryEntities(
            List<CylinderInterlockData> cylinderInterlockDataList,
            int plcId)
        {
            var memories = new List<Memory>();

            foreach (var cylinderData in cylinderInterlockDataList)
            {
                foreach (var interlockData in cylinderData.Interlocks)
                {
                    foreach (var conditionData in interlockData.Conditions)
                    {
                        if (string.IsNullOrEmpty(conditionData.Device))
                        {
                            continue;
                        }

                        var memory = new Memory
                        {
                            PlcId = plcId,
                            Device = conditionData.Device,
                            MemoryCategory = 2, // M
                            DeviceNumber = conditionData.DeviceNumber,
                            DeviceNumber1 = conditionData.DeviceNumber.ToString(),
                            DeviceNumber2 = "",
                            Category = "ｲﾝﾀﾛｯｸ",
                            Row_1 = BuildRow1(cylinderData, interlockData),
                            Row_2 = BuildRow2(conditionData),
                            Row_3 = conditionData.Condition.Name ?? "",
                            Row_4 = conditionData.Condition.Comment ?? "",
                            Direct_Input = "",
                            Confirm = "",
                            Note = "",
                            GOT = "false",
                            MnemonicId = (int)MnemonicType.CY,
                            RecordId = conditionData.Condition.CylinderId,
                            OutcoilNumber = conditionData.InterlockNumber
                        };

                        memories.Add(memory);
                    }
                }
            }

            return memories;
        }

        private static string BuildRow1(CylinderInterlockData cylinderData, InterlockData interlockData)
        {
            // シリンダー名とGoOrBack情報を組み合わせ
            var cylinderName = cylinderData.Cylinder.CYNum + cylinderData.Cylinder.CYNameSub ?? "";
            var goOrBack = interlockData.GoOrBackDisplayName;
            return $"{cylinderName}_{goOrBack}";
        }

        private static string BuildRow2(InterlockConditionData conditionData)
        {
            // 条件タイプ名または条件番号を表示
            if (conditionData.ConditionType != null)
            {
                return conditionData.ConditionType.ConditionTypeName ?? $"条件{conditionData.Condition.ConditionNumber}";
            }
            return $"条件{conditionData.Condition.ConditionNumber}";
        }
    }
}
