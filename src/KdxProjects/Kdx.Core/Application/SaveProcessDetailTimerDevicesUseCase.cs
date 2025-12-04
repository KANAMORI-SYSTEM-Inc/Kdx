using Kdx.Contracts.DTOs;
using Kdx.Contracts.Enums;
using Kdx.Core.Domain.Factories;
using Kdx.Core.Domain.Services;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kdx.Core.Application
{
    // Kdx.Core/Application/SaveProcessDetailTimerDevicesUseCase.cs
    public sealed class SaveProcessDetailTimerDevicesUseCase
    {
        private readonly IMnemonicTimerDeviceRepository _repo;
        private readonly ITimerDeviceCache _cache;
        private readonly ISequenceGenerator _seq;
        private readonly IDeviceOffsetProvider _offsets;
        private readonly bool _useCacheOnly;

        public SaveProcessDetailTimerDevicesUseCase(
            IMnemonicTimerDeviceRepository repo,
            ITimerDeviceCache cache,
            ISequenceGenerator seq,
            IDeviceOffsetProvider offsets,
            bool useCacheOnly = false)
        {
            _repo = repo;
            _cache = cache;
            _seq = seq;
            _offsets = offsets;
            _useCacheOnly = useCacheOnly;
        }

        public async Task<int> ExecuteAsync(
            IReadOnlyList<Contracts.DTOs.Timer> timers,
            IReadOnlyList<ProcessDetail> details,
            int plcId,
            CancellationToken ct = default)
        {
            var devices = new List<MnemonicTimerDevice>();

            foreach (var detail in details)
            {
                var timer = timers.FirstOrDefault(t =>
                    t.MnemonicId == (int)MnemonicType.ProcessDetail &&
                    t.CycleId == detail.CycleId);

                if (timer is null)
                {
                     continue;
                }

                var device = MnemonicTimerDeviceFactory.Create(
                    timer, detail, _seq.Next(), _offsets, plcId);

                devices.Add(device);
                _cache.AddOrUpdate(device, plcId, timer.CycleId ?? 1);
            }

            if (!_useCacheOnly && devices.Count > 0)
            {
                await _repo.AddRangeAsync(devices, ct);
            }

            return devices.Count;
        }
    }

}
