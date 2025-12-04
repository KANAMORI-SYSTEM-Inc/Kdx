using Kdx.Core.Domain.Services;
using Kdx.Infrastructure.Options;
using Microsoft.Extensions.Options;

namespace Kdx.Infrastructure.Services
{
    public sealed class DeviceOffsetProvider : IDeviceOffsetProvider
    {
        private readonly IOptions<DeviceOffsetOptions> _opt;
        
        public DeviceOffsetProvider(IOptions<DeviceOffsetOptions> opt)
        {
            _opt = opt;
        }
        
        public int DeviceStartT => _opt.Value.DeviceStartT;
        public int TimerStartZR => _opt.Value.TimerStartZR;
    }
}
