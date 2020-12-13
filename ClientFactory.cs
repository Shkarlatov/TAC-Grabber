using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;

using TAC_Grabber.Clients;

namespace TAC_Grabber
{
    static class ClientsFactory
    {
    
        private static readonly Dictionary<int, BaseHTTPClient[]> _cache = new Dictionary<int, BaseHTTPClient[]>();
        private static BaseHTTPClient[] CreateClients(HttpMessageInvoker httpClient)
        {
            return new BaseHTTPClient[]
            {
                new XinitClient(httpClient),
                new TMobileClient(httpClient),
                new ESTMobileClient(httpClient),
                new CricketWirelessClient(httpClient),
                new UScellularClient(httpClient),
                new UltraMobileClient(httpClient)
            };
        }

        public static BaseHTTPClient[] CreateClients(int proxyPort = -1)
        {
            if (_cache.TryGetValue(proxyPort, out BaseHTTPClient[]  clients))
            {
                _cache[proxyPort] = clients;
                return _cache[proxyPort];
            }
            else
            {
                IWebProxy proxy = null;
                if (proxyPort >0)
                {
                    proxy = new WebProxy("127.0.0.1", proxyPort);
                }
                SocketsHttpHandler handler = new SocketsHttpHandler
                {
                    Proxy = proxy,
                    UseProxy = (proxy != null),
                    UseCookies = false,
                    AllowAutoRedirect = false
                };
                HttpClient httpClient = new HttpClient(handler);
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (compatible;)");
                _cache[proxyPort] = CreateClients(httpClient);
                return _cache[proxyPort];
            }

        }
        public static Dictionary<string, BaseHTTPClient[]> GetClientsGroupByType()
        {
            return _cache.Values.SelectMany(x => x).GroupBy(x => x.GetType()).ToDictionary(x=>x.Key.Name,x=>x.ToArray());
        }
        public static Dictionary<int, BaseHTTPClient[]> GetClientsGroupByPort()
        {
            return _cache;
        }
    }
}
