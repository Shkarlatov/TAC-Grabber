using System.Net;
using System.Net.Http;

using TAC_Grabber.Clients;

namespace TAC_Grabber
{
    static class ClientsFactory
    {
        public static BaseHTTPClient[] GetClients(IWebProxy proxy)
        {
            var handler = new SocketsHttpHandler
            {
                Proxy = proxy,
                UseProxy = proxy != null,
                UseCookies = false,
                AllowAutoRedirect = false
            };

            var httpClient = new HttpClient(new LoggingHandler(handler));
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (compatible;)");

            return new BaseHTTPClient[]
            {
                new XinitClient(httpClient),
                new TMobileClient(httpClient),
                new ESTMobileClient(httpClient),
                new CricketWirelessClient(httpClient),
                new UScellularClient(httpClient),
                new UltraMobileClient(httpClient),

            };
        }
    }
}
