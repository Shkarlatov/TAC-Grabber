using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TAC_Grabber.Clients
{
    public class UScellularClient : BaseHTTPClient
    {
        /*
         request:      curl  -H "Authorization: UXF_SessionToken:..." https://www.uscellular.com/rp-server/commerce/v1/serial-number/356713088298045/verifyESNX9
         response:
        {
    "equipment": {
        "itemId": "BYODAPIPHN8P",
        "serialNumber": "089594752808558596",
        "attachment": [{
            "type": "image",
            "purpose": "list",
            "url": "https://www.uscellular.com/uscellular/images/logo-usc.jpg"
        }, {
    "type": "image",
            "purpose": "summary",
            "url": "https://content.uscc.com/IPHN8P/SMALLPIC.jpg"
        }, {
    "url": "Apple iPhone 8 Plus"
        }],
        "displayName": "Apple iPhone 8 Plus BYOD - New",
        "productType": "TM",
        "deviceID": "11504302_11504312",
        "equipmentCid": "11504312",
        "isHSI": false,
        "isQPP": true,
        "phoneType": "Smart"
    },
    "status": {
    "decode": "Known - Valid",
        "code": "KV"
    },
    "overrideIndicator": "No"
}
        */
        public UScellularClient(HttpMessageInvoker httpClient) : base(httpClient)
        {
        }

        protected override HttpRequestMessage GetRequestMessage(string query)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, $"https://www.uscellular.com/rp-server/commerce/v1/serial-number/{query}/verifyESNX9");
            message.Headers.TryAddWithoutValidation("Authorization", "UXF_SessionToken:XfdK1Zzl/K4UzCygbhpCyw==___currentencryptionkey___jz7o+PPu8XWOjUckJmB6IVz9YK8ofAbw66rLpNhkZhatAy1dPkbh5bko20NXcrV8a2EffrtRWYPDYZP43MWz1/D287Shws/vcxBdrUSgLdkLp2YA3xbrbgl7Hu731MBgR1adOxGrLUkoakDv3KVqhjES0f8K/I948wQZ0rY3nyM=");
            return message;
        }

        protected override async Task<string> ProcessResponseAsync(HttpResponseMessage response)
        {
            var item=await Deserialize<UScellularRootItem>(response);
            if (item != null && item.equipment != null && item.equipment.attachment!=null)
            {
                var e = item.equipment.attachment
                    .Where(x => string.IsNullOrEmpty(x.type)).First();

                var csv = new FormattedCSV();
                csv.Host = response.RequestMessage.RequestUri.Host;
                csv.TAC = response.RequestMessage.RequestUri.Segments[5].Substring(0, 8);
                csv.Result = e.url;
                return csv.ToString();
            }
            return string.Empty;
        }
    }


    public class UScellularItem
    {
        public string itemId { get; set; }
        public string serialNumber { get; set; }
        public List<Attachment> attachment { get; set; }
        public string displayName { get; set; }
        public string productType { get; set; }
        public string deviceID { get; set; }
        public string equipmentCid { get; set; }
        public bool isHSI { get; set; }
        public bool isQPP { get; set; }
        public string phoneType { get; set; }
    }

    public class UScellularRootItem
    {
        public UScellularItem equipment { get; set; }
        //public Status status { get; set; }
        public string overrideIndicator { get; set; }
    }

    public class Attachment
    {
        public string type { get; set; }
        public string purpose { get; set; }
        public string url { get; set; }
    }
}
