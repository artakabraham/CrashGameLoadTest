using CrashGameLoadTest.Factories;
using CrashGameLoadTest.HttpClient;
using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Services;
using LVC.HttpClientManager.Extensions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CrashGameLoadTest.Extensions
{
    public static class ServiceRegisterExtension
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services, IConfiguration configuration)
        {
            IConfiguration httpClientSection = configuration.GetSection("HttpClientOptions");
            services.Configure<HttpClientOptions>(httpClientSection);
            var clientConfig = httpClientSection.Get<HttpClientOptions>()!;

            services.RegisterHttpClient<IntegratorHttpClient>(new IntegratorHttpClientOptions
            {
                RetryCount = clientConfig.IntegratorHttpClientOptions!.RetryCount,
                BaseAddress = clientConfig.IntegratorHttpClientOptions.BaseAddress,
                HandlerLifetimeByMinutes = clientConfig.IntegratorHttpClientOptions.HandlerLifetimeByMinutes,
                TimeoutSeconds = clientConfig.IntegratorHttpClientOptions.TimeoutSeconds
            });

            services.AddSingleton<IPlayerPoolService, PlayerPoolService>();
            services.AddTransient<ILaunchGameService, LaunchGameService>();
            services.AddTransient<IPlayerFactory, PlayerFactory>();

            return services;
        }
    }
}