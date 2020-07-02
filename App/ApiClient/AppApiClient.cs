using System.Net.Http;

namespace ApiClient
{
    public class AppApiClient
    {
        public ApiClient.V1.IApiClient V1 { get; }
        public ApiClient.V2.IApiClient V2 { get; }

        public AppApiClient(HttpClient httpClient)
        {
            V1 = new ApiClient.V1.ApiClient(httpClient);
            V2 = new ApiClient.V2.ApiClient(httpClient);
        }
    }
}
