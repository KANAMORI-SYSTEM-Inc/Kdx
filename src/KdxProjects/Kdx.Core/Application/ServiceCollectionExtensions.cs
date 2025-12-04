using Kdx.Contracts.Interfaces;
using Kdx.Core.Application.Strategies;
using Kdx.Infrastructure.Supabase.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace Kdx.Core.Application
{
    /// <summary>
    /// DI コンテナへの登録拡張メソッド
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// ErrorAggregator サービスを登録
        /// </summary>
        public static IServiceCollection AddErrorAggregator(this IServiceCollection services, int? defaultMnemonicId = null)
        {
            services.AddScoped<IErrorAggregator>(provider => new ErrorAggregator(defaultMnemonicId));
            return services;
        }

        /// <summary>
        /// IOAddressService を登録
        /// </summary>
        public static IServiceCollection AddIOAddressService(
            this IServiceCollection services,
            int plcId)
        {
            services.AddScoped<IIOAddressService>(provider =>
            {
                var errorAggregator = provider.GetRequiredService<IErrorAggregator>();
                var repository = provider.GetRequiredService<ISupabaseRepository>();
                var ioSelectorService = provider.GetRequiredService<IIOSelectorService>();
                return new IOAddressService(errorAggregator, repository, plcId, ioSelectorService);
            });
            return services;
        }
        /// <summary>
        /// InterlockMnemonicOutput関連のサービスを登録
        /// </summary>
        public static IServiceCollection AddInterlockMnemonicOutputs(this IServiceCollection services)
        {
            // 各Strategyを登録
            services.AddSingleton<IInterlockMnemonicStrategy, On1Strategy>();
            services.AddSingleton<IInterlockMnemonicStrategy, On2Strategy>();
            services.AddSingleton<IInterlockMnemonicStrategy, OnMStrategy>();
            services.AddSingleton<IInterlockMnemonicStrategy, OnOrStrategy>();
            services.AddSingleton<IInterlockMnemonicStrategy, Off1Strategy>();

            // 将来的にOn3Strategy, On4Strategy...を追加する場合はここに追加

            // Strategy Resolverを登録
            services.AddSingleton<IInterlockMnemonicStrategyResolver, InterlockMnemonicStrategyResolver>();

            // メインのOutput実装を登録
            services.AddSingleton<IInterlockMnemonicOutput, InterlockMnemonicOutput>();

            return services;
        }

        /// <summary>
        /// カスタムStrategyを追加登録する拡張メソッド
        /// </summary>
        /// <typeparam name="TStrategy">追加するStrategy型</typeparam>
        public static IServiceCollection AddInterlockMnemonicStrategy<TStrategy>(this IServiceCollection services)
            where TStrategy : class, IInterlockMnemonicStrategy
        {
            services.AddSingleton<IInterlockMnemonicStrategy, TStrategy>();
            return services;
        }
    }
}
