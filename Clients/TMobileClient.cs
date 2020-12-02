using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TAC_Grabber.Clients
{
    public class TMobileClient : BaseHTTPClient
    {
        protected string UrlAccessToken = "https://join.t-mobile.com/api/access_token";
        protected string UrlCheckIMEI = "https://join.t-mobile.com/api/get_byod_check?imeiNumber=";

        private string authorization = string.Empty;

        public TMobileClient(HttpMessageInvoker httpClient) : base(httpClient) { }

        protected override HttpRequestMessage GetRequestMessage(string query)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, UrlCheckIMEI + query);
            message.Headers.TryAddWithoutValidation("Authorization",authorization);

            return message;

        }

        protected override async Task ReAuthAsync()
        {
            var message = new HttpRequestMessage(HttpMethod.Get, UrlAccessToken);
            var response =await client.SendAsync(message, default);
            if(response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                try
                {
                    authorization= JsonDocument
                        .Parse(json)
                        .RootElement
                        .GetProperty("access_token")
                        .GetString();
                }
                catch { }
            }
        }
        protected override async Task<string> ProcessResponseAsync(HttpResponseMessage response)
        {
            var items=await Deserialize<TMobileResponse[]>(response);

            var sb = new StringBuilder();
            foreach(var item in items
                .Where(x=>
                !string.IsNullOrEmpty(x.Manufacturer) && 
                !string.IsNullOrEmpty(x.MarketingName)))
            {
                var csv = new FormattedCSV();
                csv.Host = response.RequestMessage.RequestUri.Host;
                csv.TAC = item.IMEI.Substring(0, 8);
                csv.Result = $"{item.Manufacturer} {item.MarketingName}";

                if (csv.Result.Contains("NotFound"))
                    continue;

                sb.AppendLine(csv.ToString());
            }
            return sb.ToString();
        }
    }

    public class TMobileResponse
    {
        public string IMEI { get; set; }
        public string Compatibility { get; set; }
        public string Manufacturer { get; set; }
        public string MarketingName { get; set; }
        public bool isL700TmoSupported { get; set; }
        public bool isVoLTETmoSupported { get; set; }
        public bool CompatibilityServiceUp { get; set; }
        public bool EligibilityStatusFound { get; set; }
        public bool EligibilityServiceUp { get; set; }
    }
}
