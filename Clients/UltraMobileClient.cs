using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace TAC_Grabber.Clients
{
    /*
     requst: curl -d "{\"device\":{\"tac\":\"35931308\"}}"
    -H "Content-Type: application/json"
    -X POST https://w3b-api.ultramobile.com/v1/network/check

    response:
         {
    "device": {
        "bands": {
            "_4G": [2, 4, 5, 12],
            "_3G": [2, 4]
        },
        "name": "Apple iPhone 8 Plus",
        "isMobileDevice": true,
        "volteCompatible": true,
        "compatibility": "COMPATIBLE"
    }}


    */
    public class UltraMobileClient : BaseHTTPClient
    {
        public UltraMobileClient(HttpMessageInvoker httpClient) : base(httpClient)
        {
        }

        protected override HttpRequestMessage GetRequestMessage(string query)
        {
            var message = new HttpRequestMessage(HttpMethod.Post, "https://w3b-api.ultramobile.com/v1/network/check");
            //var "{\"device\":{\"tac\":\"{0} \"}}";
            var tac = new UltraMobileJson(query.Substring(0, 8));
            var str=JsonSerializer.Serialize<UltraMobileJson>(tac);

            message.Content = new StringContent(str, Encoding.UTF8, "application/json");
            return message;
        }

        protected override async Task<string> ProcessResponseAsync(HttpResponseMessage response)
        {
            var item=await Deserialize<UltraMobileRootItem>(response);
            if(item!=null && item.device!=null)
            {
               var json= await response.RequestMessage.Content.ReadAsStringAsync();
                var tac=JsonSerializer.Deserialize<UltraMobileJson>(json);

                var csv = new FormattedCSV();
                csv.Host = response.RequestMessage.RequestUri.Host;
                csv.TAC = tac.device.tac;
                csv.Result = item.device.name;
                return csv.ToString();
            }
            return string.Empty;
        }
    }

    public class UltraMobileItem
    {
        //public Bands bands { get; set; }
        public string name { get; set; }
        public bool isMobileDevice { get; set; }
        public bool volteCompatible { get; set; }
        public string compatibility { get; set; }
    }

    public class UltraMobileRootItem
    {
        public UltraMobileItem device { get; set; }
    }

    public class UltraMobileJson
    {
        public  Device device { get; set; }

        public UltraMobileJson() { }
        public UltraMobileJson(string tac)
        {
            device = new Device();
            device.tac = tac;
        }
    }

    public class Device
    {
        public string tac { get; set; }
    }

}
