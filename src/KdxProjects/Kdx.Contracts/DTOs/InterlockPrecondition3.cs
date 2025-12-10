namespace Kdx.Contracts.DTOs
{
    /// <summary>
    /// インターロック前提条件3 DTO
    /// 特定IOまたは特定デバイスがONしている場合にインターロックが有効になる条件
    /// </summary>
    public class InterlockPrecondition3
    {
        public int Id { get; set; }

        /// <summary>
        /// 条件タイプ（"IO" または "Device"）
        /// </summary>
        public string? ConditionType { get; set; }

        /// <summary>
        /// IOアドレス（ConditionType="IO"の場合に使用）
        /// </summary>
        public string? IOAddress { get; set; }

        /// <summary>
        /// デバイスアドレス（ConditionType="Device"の場合に使用）
        /// </summary>
        public string? DeviceAddress { get; set; }

        /// <summary>
        /// ON条件かどうか（true=ON時に有効、false=OFF時に有効）
        /// </summary>
        public bool IsOnCondition { get; set; } = true;

        /// <summary>
        /// 説明・コメント
        /// </summary>
        public string? Description { get; set; }
    }
}
