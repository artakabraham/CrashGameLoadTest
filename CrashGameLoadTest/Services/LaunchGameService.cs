using CrashGameLoadTest.HttpClient;
using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;
using System.Web;

namespace CrashGameLoadTest.Services
{
    public class LaunchGameService : ILaunchGameService
    {
        private readonly IntegratorHttpClient _integratorHttpClient;

        public LaunchGameService(IntegratorHttpClient integratorHttpClient)
        {
            _integratorHttpClient = integratorHttpClient;
        }


        public async Task<string> LaunchGameAsync(PlayerPoolItem player, CancellationToken cancellationToken)
        {
            var launchUrl = await _integratorHttpClient.LaunchGameAsync(player, cancellationToken);

            var jwtToken = ExtractJwt(launchUrl.Data.RealUri);

            return jwtToken;
        }

        private static string ExtractJwt(string url)
        {
            var query = new Uri(url).Query;
            return HttpUtility.ParseQueryString(query).Get("token");
        }
    }
}