using Floai.ApiClients.abs;
using Floai.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Floai.ApiClients
{
    public class HttpApiClient : BaseApiClient
    {
        private readonly HttpApiClientOptions options;

        static readonly HttpClient httpClient = new HttpClient();
        public HttpApiClient(HttpApiClientOptions options) : base(options)
        {
            this.options = options;
        }

        public async override Task CreateCompletionAsync(List<ChatMessage> messages, Action<string, bool> onDataReceived)
        {
            //URL
            var uriBuilder = new UriBuilder(options.Url);
            if(options.Params != null)
            {
                var query = new StringBuilder();
                foreach (var param in options.Params)
                {
                    if (query.Length > 0)
                        query.Append("&");

                    query.Append(Uri.EscapeDataString(param.Key));
                    query.Append("=");
                    query.Append(Uri.EscapeDataString(param.Value));
                }
                uriBuilder.Query = query.ToString();
                
            }
            var fullUrl = uriBuilder.ToString();

            //Header
            foreach (var header in options.Headers)
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }

            //Body
            string replacedBodyValue = options.Body.Replace("${content}", messages.LastOrDefault().Content);
            StringContent content = new StringContent(replacedBodyValue, Encoding.UTF8, "application/json");


            HttpResponseMessage response = await httpClient.PostAsync(fullUrl, content);

            string responseContent;
            using (JsonDocument document = JsonDocument.Parse(await response.Content.ReadAsStringAsync()))
            {
                JsonElement root = document.RootElement;

                JsonElement choiceElement = root.GetProperty("choices")[0];

                JsonElement messageElement = choiceElement.GetProperty("message");

                responseContent = messageElement.GetProperty("content").GetString();
            }

            onDataReceived(responseContent, true);
        }
    }
}
