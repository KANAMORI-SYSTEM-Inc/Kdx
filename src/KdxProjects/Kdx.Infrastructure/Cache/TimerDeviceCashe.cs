using Kdx.Contracts.DTOs;
using Kdx.Core.Domain.Services;
using Kdx.Core.Domain.Interfaces;

namespace Kdx.Infrastructure.Cache
{
    public sealed class TimerDeviceCache : ITimerDeviceCache
    {
        private readonly IMnemonicDeviceMemoryStore _memoryStore;
        
        public TimerDeviceCache(IMnemonicDeviceMemoryStore memoryStore)
        {
            _memoryStore = memoryStore;
        }
        
        public void AddOrUpdate(MnemonicTimerDevice device, int plcId, int cycleId)
            => _memoryStore.AddOrUpdateTimerDevice(device, plcId, cycleId);
    }
}
