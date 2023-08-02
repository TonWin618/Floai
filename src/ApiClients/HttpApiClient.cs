using Floai.ApiClients.abs;
using Floai.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Floai.ApiClients;

public class HttpApiClient : BaseApiClient
{
    private readonly HttpApiClientOptions options;

    private readonly HttpClient httpClient = new HttpClient();
    public HttpApiClient(HttpApiClientOptions options) : base(options)
    {
        this.options = options;

        //Header
        foreach (var header in options.Headers)
        {
            httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
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

        //Body
        StringBuilder history = new StringBuilder();
        foreach(var message in messages.SkipLast(1))
        {
            history.Append(options.HistoryFormat
                .Replace("${sender}", message.Sender == Sender.AI ? options.AiRoleName : options.UserRoleName)
                .Replace("${content}", message.Content)
                );
        }

        string requestBody = options.Body
            .Replace("${history}", history.ToString())
            .Replace("${content}", messages.LastOrDefault().Content);
        StringContent requestContent = new StringContent(requestBody, Encoding.UTF8, "application/json");

        //Request
        try
        {
            HttpResponseMessage response = await httpClient.PostAsync(fullUrl, requestContent);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();

                JsonDocument jsonDocument = JsonDocument.Parse(responseContent);
                JsonElement root = jsonDocument.RootElement;
                string messageContent = GetValueFromPath(root, options.ContentPath);
                onDataReceived(messageContent, true);
            }
            else
            {
                onDataReceived($"StatusCode: {response.StatusCode}\n Reason: {response.ReasonPhrase} \n Content: {await response.Content.ReadAsStringAsync()}", false);
            }
        }
        catch (HttpRequestException ex)
        {
            onDataReceived(ex.Message, false);
        }
    }

    public static string GetValueFromPath(JsonElement root, string path)
    {
        string[] parts = path.Split('/');

        JsonElement currentElement = root;
        for (int i = 0; i < parts.Length; i++)
        {
            string part = parts[i];
            if (int.TryParse(part, out int index))
            {
                currentElement = currentElement[index];
            }
            else
            {
                currentElement = currentElement.GetProperty(part);
            }
        }

        return currentElement.GetString();
    }
}

