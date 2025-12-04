namespace Kdx.Infrastructure.Configuration
{
    /// <summary>
    /// Supabase接続設定
    /// </summary>
    public class SupabaseConfiguration
    {
        /// <summary>
        /// SupabaseプロジェクトのURL
        /// </summary>
        public string Url { get; set; } = string.Empty;
        
        /// <summary>
        /// Supabaseの匿名キー
        /// </summary>
        public string AnonKey { get; set; } = string.Empty;
        
        /// <summary>
        /// タイムアウト秒数
        /// </summary>
        public int TimeoutSeconds { get; set; } = 30;
    }
}