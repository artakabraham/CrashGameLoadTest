using LVC.HttpClientManager.Abstractions;

namespace CrashGameLoadTest.HttpClient
{
    public class IntegratorHttpClient : HttpClientManagerBase
    {
        public IntegratorHttpClient(System.Net.Http.HttpClient httpClient) : base(httpClient)
        {
        }
    }
}