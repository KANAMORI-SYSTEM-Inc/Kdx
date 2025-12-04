using Kdx.Contracts.DTOs;

namespace Kdx.Core.Domain.Interfaces
{
    public interface IMnemonicDeviceMemoryStore
    {
        void AddOrUpdateTimerDevice(MnemonicTimerDevice device, int plcId, int cycleId);
    }
}