using CrashGameLoadTest.Interfaces;
using CrashGameLoadTest.Models;
using LVC.SharedModels.Response;
using System.Text.Json;
using System.Web;

namespace CrashGameLoadTest.Actions
{
    public class LaunchGameAction : IPlayerAction
    {
        private readonly string _integratorUrl;
        private readonly LaunchGameRequestModel _launchRequestModel;

        public LaunchGameAction(string integratorUrl, LaunchGameRequestModel launchGameRequestModel)
        {
            _integratorUrl = integratorUrl;
            _launchRequestModel = launchGameRequestModel;
        }

        public async Task ExecuteAsync(PlayerContext context, CancellationToken token)
        {
            try
            {
                var jsonStringData = JsonSerializer.Serialize(_launchRequestModel);

                var responseJson = await context.HttpClient.SendAsync(
                    jsonStringData: jsonStringData,
                    relativeUri: _integratorUrl,
                    httpMethod: HttpMethod.Post,
                    enableCompression: true,
                    cancellationToken: token);

                if (string.IsNullOrEmpty(responseJson))
                {
                    throw new Exception("Failed to execute the HTTP request.");
                }

                var response = JsonSerializer.Deserialize<ResultResponse<LaunchGameResponseModel>>(responseJson);
                context.JwtToken = ExtractJwt(response.Data.RealUri);

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }


        }

        private static string ExtractJwt(string url)
        {
            var query = new Uri(url).Query;
            var token = HttpUtility.ParseQueryString(query).Get("token");
            return token ?? throw new InvalidOperationException("Token not found in LaunchURL");
        }
    }
}