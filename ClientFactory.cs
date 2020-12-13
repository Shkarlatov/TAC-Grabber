using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection;
using TAC_Grabber.Clients;

namespace TAC_Grabber
{
    static class ClientsFactory
    {

        static readonly Dictionary<int, BaseHTTPClient[]> _cache=new Dictionary<int,BaseHTTPClient[]>();

        static BaseHTTPClient[] CreateClients(HttpMessageInvoker httpClient)
        {
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

        public static Dictionary<int,BaseHTTPClient[]> GetClientsByProxyPort(int port)
        {
            return _cache;
        }

        public static Dictionary<Type,BaseHTTPClient[]> GetGroupClientsByType()
        {
            return _cache.Values.SelectMany(x => x).GroupBy(x => x.GetType()).ToDictionary(x => x.Key, x => x.ToArray());
        }

        public static BaseHTTPClient[] CreateClients(int proxyPort=-1)
        {

            if(_cache.TryGetValue(proxyPort,out BaseHTTPClient[] clients))
            {
                return clients;
            }
            else
            {
                IWebProxy proxy = null;
                if (proxyPort > 0)
                {
                    proxy = new WebProxy("127.0.0.1", proxyPort);
                }

                var handler = new SocketsHttpHandler
                {
                    Proxy = proxy,
                    UseProxy = proxy != null,
                    UseCookies = false,
                    AllowAutoRedirect = false
                };

                //var httpClient = new HttpClient(new LoggingHandler(handler));

                var httpClient = new HttpClient(handler);


                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0 (compatible;)");

                _cache[proxyPort]=CreateClients(httpClient);
                return _cache[proxyPort];
            }
        }
    }
}
