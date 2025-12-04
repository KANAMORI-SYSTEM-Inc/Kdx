using Microsoft.Extensions.Configuration;
using Supabase;

namespace Kdx.Infrastructure.Supabase.Tests;

/// <summary>
/// テストベースクラス - Supabaseクライアントの初期化を提供
/// </summary>
public class TestBase : IDisposable
{
    protected readonly Client SupabaseClient;
    protected readonly IConfiguration Configuration;

    public TestBase()
    {
        // appsettings.test.json から設定を読み込み
        Configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.test.json", optional: false)
            .AddEnvironmentVariables()
            .Build();

        var supabaseUrl = Configuration["Supabase:Url"]
            ?? throw new InvalidOperationException("Supabase:Url not configured in appsettings.test.json");
        var supabaseKey = Configuration["Supabase:Key"]
            ?? throw new InvalidOperationException("Supabase:Key not configured in appsettings.test.json");

        var options = new SupabaseOptions
        {
            AutoRefreshToken = true,
            AutoConnectRealtime = false // テストではRealtimeは不要
        };

        SupabaseClient = new Client(supabaseUrl, supabaseKey, options);
    }

    public void Dispose()
    {
        // クリーンアップ処理
        GC.SuppressFinalize(this);
    }
}
