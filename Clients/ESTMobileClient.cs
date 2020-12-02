using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace TAC_Grabber.Clients
{
    public class ESTMobileClient : TMobileClient
    {
        public ESTMobileClient(HttpMessageInvoker httpClient) : base(httpClient)
        {
            UrlAccessToken = "https://es.t-mobile.com/sdjoin/api/access_token";
            UrlCheckIMEI = "https://es.t-mobile.com/sdjoin/api/get_byod_check?imeiNumber=";
        }
    }
}
