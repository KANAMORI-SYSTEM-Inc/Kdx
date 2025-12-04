using Kdx.Infrastructure.Configuration;
using Kdx.Infrastructure.Supabase.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Supabase;

namespace Kdx.Infrastructure.Extensions
{
    /// <summary>
    /// DIコンテナ登録用の拡張メソッド
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Supabase関連のサービスをDIコンテナに登録
        /// </summary>
        public static IServiceCollection AddSupabaseServices(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Supabase設定を登録
            var supabaseSettings = configuration.GetSection("Supabase").Get<SupabaseSettings>()
                ?? throw new InvalidOperationException("Supabase settings not found in configuration");

            services.AddSingleton(supabaseSettings);

            // Supabaseクライアントを登録
            services.AddSingleton<Client>(provider =>
            {
                var settings = provider.GetRequiredService<SupabaseSettings>();
                var options = new SupabaseOptions
                {
                    AutoConnectRealtime = true,
                    AutoRefreshToken = true
                };

                var client = new Client(settings.Url, settings.AnonKey, options);
                client.InitializeAsync().GetAwaiter().GetResult();
                return client;
            });

            // リポジトリを登録
            services.AddScoped<ISupabaseRepository, SupabaseRepository>();

            return services;
        }
    }
}