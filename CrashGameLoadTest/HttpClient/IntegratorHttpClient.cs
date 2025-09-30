using System.Text.Json;
using CrashGameLoadTest.Models;
using LVC.HttpClientManager.Abstractions;
using LVC.SharedModels.Response;

namespace CrashGameLoadTest.HttpClient
{
    public class IntegratorHttpClient : HttpClientManagerBase
    {
        public IntegratorHttpClient(System.Net.Http.HttpClient httpClient) : base(httpClient)
        {
        }

        public async Task<ResultResponse<LaunchGameResponseModel>> LaunchGameAsync(
            PlayerPoolItem player,
            CancellationToken cancellationToken)
        {
            try
            {
                var launchRequest = new LaunchGameRequestModel
                {
                    B2BToken = player.B2BToken,
                    PlatformId = player.PlatformId,
                    PartnerId = player.PartnerId,
                    PlayerId = player.PlayerId,
                    Currency = player.Currency,
                    GameVersion = "1.0",
                    GameId = "7",
                    Language = "EN",
                    Balance = player.Balance,
                    NickName = player.NickName,
                    IsDemo = player.IsDemo
                };

                var jsonStringData = JsonSerializer.Serialize(launchRequest);

                var responseJson = await SendAsync(
                    jsonStringData: jsonStringData,
                    relativeUri: "api/v1/Integrator/LaunchGame",
                    httpMethod: HttpMethod.Post,
                    enableCompression: false,
                    cancellationToken: cancellationToken);

                if (string.IsNullOrEmpty(responseJson))
                {
                    throw new Exception($"Failed to launch game for player {player.PlayerId}");
                }

                var response = JsonSerializer.Deserialize<ResultResponse<LaunchGameResponseModel>>(responseJson, JsonSerializerOptions);

                if (response is null)
                {
                    throw new Exception("Failed to execute the HTTP request.");
                }

                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw;
            }
        }
    }
}