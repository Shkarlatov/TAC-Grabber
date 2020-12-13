using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TAC_Grabber.Clients
{
    /*
    request: curl https://www.cricketwireless.com/restservices/onlineadapter/v1/devices/359313080018686
    response:
    {
    "data": {
        "make": "OUKITEL K6000 Plus",
        "model": "K6000 Plus",
        "mht": "NONMHT",
        "blockedDevice": false,
        "ntCompatible": true,
        "authorized": true,
        "dataOnly": false,
        "lte": true,
        "volte": false,
        "futureBlocked": false
    },
    "notifications": {
        "errors": [],
        "warnings": []
    },
    "success": true
}

     */
    public class CricketWirelessClient : BaseHTTPClient
    {
        public CricketWirelessClient(HttpMessageInvoker httpClient) : base(httpClient)
        {
        }

        protected override HttpRequestMessage GetRequestMessage(string query)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, "https://www.cricketwireless.com/restservices/onlineadapter/v1/devices/" + query);
            return message;
        }

        protected override async Task<string> ProcessResponseAsync(HttpResponseMessage response)
        {
            var item=await Deserialize<CricketWirelessRootItem>(response);

            if (item != null && item.Data != null)
            {
                var data = item.Data;
                var csv = new FormattedCSV();
                csv.Host = response.RequestMessage.RequestUri.Host;
                csv.TAC = response.RequestMessage.RequestUri.Segments.Last().Substring(0,8);
                if (data.Make == data.Model)
                    csv.Result = data.Make;
                else
                    csv.Result = $"{data.Make} {data.Model}";
                return csv.ToString();
            }
            return string.Empty;
        }
    }


    public class CricketWirelessRootItem
    {
        public CricketWirelessItem Data { get; set; }
    }
    public class CricketWirelessItem
    {
        public string Make { get; set; }
        public string Model { get; set; }
        public string mht { get; set; }
        public bool BlockedDevice { get; set; }
        public bool ntCompatible { get; set; }
        public bool Authorized { get; set; }
        public bool DataOnly { get; set; }
        public bool LTE { get; set; }
        public bool VoLTE { get; set; }
        public bool FutureBlocked { get; set; }
    }
}
