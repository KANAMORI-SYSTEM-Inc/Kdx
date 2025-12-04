namespace Kdx.Infrastructure.Configuration
{
    /// <summary>
    /// Supabase接続設定
    /// </summary>
    public class SupabaseSettings
    {
        /// <summary>
        /// SupabaseプロジェクトのURL
        /// </summary>
        public string Url { get; set; } = string.Empty;

        /// <summary>
        /// Supabaseのanon key
        /// </summary>
        public string AnonKey { get; set; } = string.Empty;

        /// <summary>
        /// タイムアウト設定（秒）
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}