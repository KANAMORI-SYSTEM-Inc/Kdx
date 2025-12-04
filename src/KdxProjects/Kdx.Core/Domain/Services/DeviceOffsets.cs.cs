using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Kdx.Contracts.DTOs;


namespace Kdx.Core.Domain.Services
{
    // Kdx.Core/Domain/Services/DeviceOffsets.cs
    public interface IDeviceOffsetProvider
    {
        int DeviceStartT { get; }    // 例: ST/T の開始番号
        int TimerStartZR { get; }    // 例: ZR のオフセット
    }

    public interface ISequenceGenerator
    {
        int Next();                  // 連番採番（必要なら plcId 単位で実装側が分離）
    }

    public interface ITimerDeviceCache
    {
        void AddOrUpdate(MnemonicTimerDevice device, int plcId, int cycleId);
    }

    public interface IMnemonicTimerDeviceRepository
    {
        Task AddRangeAsync(IEnumerable<MnemonicTimerDevice> devices, CancellationToken ct);
    }

}
