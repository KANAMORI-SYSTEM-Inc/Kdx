using Kdx.Contracts.DTOs;
using Kdx.Core.Domain.Interfaces;

namespace Kdx.Infrastructure.Cache
{
    public class MnemonicDeviceMemoryStore : IMnemonicDeviceMemoryStore
    {
        private readonly object _lock = new();
        private readonly Dictionary<int, Dictionary<int, List<MnemonicTimerDevice>>> _timerDevices;

        public MnemonicDeviceMemoryStore()
        {
            _timerDevices = new Dictionary<int, Dictionary<int, List<MnemonicTimerDevice>>>();
        }

        public void AddOrUpdateTimerDevice(MnemonicTimerDevice device, int plcId, int cycleId)
        {
            lock (_lock)
            {
                if (!_timerDevices.ContainsKey(plcId))
                {
                    _timerDevices[plcId] = new Dictionary<int, List<MnemonicTimerDevice>>();
                }

                if (!_timerDevices[plcId].ContainsKey(cycleId))
                {
                    _timerDevices[plcId][cycleId] = new List<MnemonicTimerDevice>();
                }

                var existingIndex = _timerDevices[plcId][cycleId].FindIndex(d =>
                    d.MnemonicId == device.MnemonicId &&
                    d.RecordId == device.RecordId &&
                    d.TimerId == device.TimerId);

                if (existingIndex >= 0)
                {
                    _timerDevices[plcId][cycleId][existingIndex] = device;
                }
                else
                {
                    _timerDevices[plcId][cycleId].Add(device);
                }
            }
        }
    }
}
