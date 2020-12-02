using System;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

using Polly;
using Polly.Wrap;

namespace TAC_Grabber
{
    public abstract class BaseHTTPClient
    {
        protected readonly HttpMessageInvoker client;

        AsyncPolicyWrap<HttpResponseMessage> policyWrap;

        public BaseHTTPClient(HttpMessageInvoker httpClient)
        {
            if (httpClient == null)
                throw new ArgumentNullException(nameof(httpClient));

            client = httpClient;

            var retryPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => !r.IsSuccessStatusCode)
                .WaitAndRetryForeverAsync(retryAttemp => TimeSpan.FromSeconds(Math.Pow(2, retryAttemp)));

            var reauthPolicy = Policy
                .HandleResult<HttpResponseMessage>(r => r.StatusCode == HttpStatusCode.Unauthorized)
                .RetryAsync(
                retryCount: 1,
                onRetryAsync: (outcome, retryNumber, context) => ReAuthAsync()
                );

            policyWrap = Policy.WrapAsync<HttpResponseMessage>(retryPolicy, reauthPolicy);
        }


        public async Task<string> GetQueryAsync(string query)
        {
            var response = await QueryAsync(query);
            return await ProcessResponseAsync(response);
        }



        private static JsonSerializerOptions serializerOptions = new JsonSerializerOptions
        {
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = true,
            AllowTrailingCommas = true,
        };

        protected async Task<T> Deserialize<T>(HttpResponseMessage responseMessage)
        {
            T value = default;

            if (responseMessage.IsSuccessStatusCode)
            {
                var stream = await responseMessage.Content.ReadAsStreamAsync();
                try
                {
                    value = await JsonSerializer.DeserializeAsync<T>(stream, serializerOptions);
                }
                catch { }
            }
            return value;
        }



        protected Task<HttpResponseMessage> QueryAsync(string query)
        {
            return policyWrap.ExecuteAsync(() =>
            {
                var message = GetRequestMessage(query);
                return client.SendAsync(message, default);
            });
        }

        protected virtual Task ReAuthAsync()
        {
            return Task.CompletedTask;
        }

        protected abstract HttpRequestMessage GetRequestMessage(string query);
        protected abstract Task<string> ProcessResponseAsync(HttpResponseMessage response);
    }
}
