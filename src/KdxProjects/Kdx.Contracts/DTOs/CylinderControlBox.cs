namespace Kdx.Contracts.DTOs
{
    public class CylinderControlBox
    {
        public int CylinderId { get; set; }
        public int PlcId { get; set; }  // デフォルト値を0に設定
        public int ManualNumber { get; set; }
        public string? Device { get; set; } = string.Empty;
    }
}
