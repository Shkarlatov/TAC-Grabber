using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace TAC_Grabber.Clients
{
    /*
     
    request: curl -H "referer: https://xinit.ru/" -H "accept-language: ru") https://xinit.ru/api/imei/356713088298045
    response:
    [{
    "tac": "35",
    "info": "Group/indication: Comreg; Origin: Ireland"},
    {
    "tac": "35671308",
    "info": "Apple iPhone 8 Plus (A1864)\r"}]

    */
    public class XinitClient : BaseHTTPClient
    {
        public XinitClient(HttpMessageInvoker httpClient) : base(httpClient) { }

        protected override HttpRequestMessage GetRequestMessage(string imei)
        {
            var message = new HttpRequestMessage(HttpMethod.Get, "https://xinit.ru/api/imei/" + imei);
            message.Headers.TryAddWithoutValidation("referer", "https://xinit.ru/imei/");
            message.Headers.TryAddWithoutValidation("accept-language", "ru");
            return message;
        }

        protected override async Task<string> ProcessResponseAsync(HttpResponseMessage response)
        {
            var items = await Deserialize<XinitResponse[]>(response);


            var sb = new StringBuilder();
            foreach (var item in items.Where(x => x.TAC.Length == 8))
            {
                var cvs = new FormattedCVS();
                cvs.Host = response.RequestMessage.RequestUri.Host;
                cvs.TAC = item.TAC;
                cvs.Result = item.Info;

                sb.AppendLine(cvs.ToString());

            }
            return sb.ToString();

        }
    }

    public class XinitResponse
    {
        public string TAC { get; set; }
        public string Info { get; set; }
    }
}
