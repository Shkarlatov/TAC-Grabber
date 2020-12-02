using System.Net;
using System.Net.Http;

using TAC_Grabber.Clients;

namespace TAC_Grabber
{
    static class ClientsFactory
    {
        public static BaseHTTPClient[] GetClients(IWebProxy proxy)
        {
            var handler = new HttpClientHandler
            {
                Proxy = proxy,
                UseProxy = proxy != null,
                UseCookies = false,
                AllowAutoRedirect = false
            };

            var httpClient = new HttpClient(handler);
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (compatible;)");

            return new[]
            {
                new XinitClient(httpClient)
            };
        }
    }
}
