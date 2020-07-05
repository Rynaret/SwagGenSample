using System.Net.Http;

namespace ApiClient
{
    public class AppApiClient
    {
        public IV1Client V1 { get; }
        public IV2Client V2 { get; }

        public AppApiClient(HttpClient httpClient)
        {
            V1 = new V1Client(httpClient);
            V2 = new V2Client(httpClient);
        }
    }
}