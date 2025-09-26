using System.Net.Http.Json;
using System.Web;
using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;

namespace CrashGameLoadTest.Actions
{
    public class AuthenticateAction : IPlayerAction
    {
        private readonly string _integratorUrl;
        private readonly string _username;
        private readonly string _password;
        
        public AuthenticateAction(string integratorUrl, string username, string password)
        {
            _integratorUrl = integratorUrl;
            _username = username;
            _password = password;
        }
        
        public async Task ExecuteAsync(PlayerContext context, CancellationToken token)
        {
            var response = await context.HttpClient.PostAsJsonAsync(
                $"{_integratorUrl}/auth",
                new { username = _username, password = _password },
                token);

            response.EnsureSuccessStatusCode();

            var launch = await response.Content.ReadFromJsonAsync<LaunchResponseModel.LaunchResponse>(cancellationToken: token);
            context.JwtToken = ExtractJwt(launch!.LaunchUrl);
        }
        
        private static string ExtractJwt(string url)
        {
            var query = new Uri(url).Query;
            var token = HttpUtility.ParseQueryString(query).Get("token");
            return token ?? throw new InvalidOperationException("Token not found in LaunchURL");
        }
    }
}