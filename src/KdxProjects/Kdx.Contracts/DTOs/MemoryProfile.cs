namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// メモリプロファイルDTO
    /// デバイス開始アドレスの設定を管理
    /// CycleIdが主キー
    /// </summary>
    public class MemoryProfile
    {
        public int CycleId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public bool IsDefault { get; set; }

        // デバイス開始アドレス設定
        public int ProcessDeviceStartL { get; set; }
        public int DetailDeviceStartL { get; set; }
        public int OperationDeviceStartM { get; set; }
        public int CylinderDeviceStartM { get; set; }
        public int CylinderDeviceStartD { get; set; }
        public int ErrorDeviceStartM { get; set; }
        public int ErrorDeviceStartT { get; set; }
        public int DeviceStartT { get; set; }
        public int TimerStartZR { get; set; }
        public int ProsTimeStartZR { get; set; }
        public int ProsTimePreviousStartZR { get; set; }
        public int CyTimeStartZR { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
