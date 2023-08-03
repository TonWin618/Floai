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
        if (options.Headers != null)
        {
            foreach (var header in options.Headers)
            {
                httpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
            }
        }
    }

    public async override Task CreateCompletionAsync(List<ChatMessage> messages, Action<string, bool> onDataReceived)
    {
        //Content
        StringContent requestContent = new StringContent(GenerateRequestBody(messages), Encoding.UTF8, "application/json");

        //Request
        string content;
        bool isSaved;

        try
        {
            HttpResponseMessage response = await httpClient.PostAsync(BuildCompleteUrl(), requestContent);

            if (response.IsSuccessStatusCode)
            {
                string responseContent = await response.Content.ReadAsStringAsync();
                try
                {
                    JsonDocument jsonDocument = JsonDocument.Parse(responseContent);
                    JsonElement root = jsonDocument.RootElement;

                    content = GetValueFromPath(root, options.ContentPath);
                    isSaved = true;
                }
                catch(Exception ex)
                {
                    content = ex.Message;
                    isSaved = false;
                }
            }
            else
            {
                content = $"StatusCode: {response.StatusCode}\nReason: {response.ReasonPhrase} \nContent: {await response.Content.ReadAsStringAsync()}";
                isSaved = false ;
            }
        }
        catch (HttpRequestException ex)
        {
            content = ex.Message;
            isSaved = false;
        }

        onDataReceived(content, isSaved);
    }

    public string BuildCompleteUrl()
    {
        var uriBuilder = new UriBuilder(options.Url);
        if (options.Params != null)
        {
            var query = new StringBuilder();
            foreach (var param in options.Params)
            {
                if (query.Length > 0)
                    query.Append('&');

                query.Append(Uri.EscapeDataString(param.Key));
                query.Append('=');
                query.Append(Uri.EscapeDataString(param.Value));
            }
            uriBuilder.Query = query.ToString();
        }
        return uriBuilder.ToString();
    }

    public string GetValueFromPath(JsonElement root, string path)
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

    public string GenerateRequestBody(List<ChatMessage> messages)
    {
        StringBuilder history = new StringBuilder();
        if (options.HistoryTemplate.Contains("${sender}"))
        {
            foreach (var message in messages.SkipLast(1))
            {
                history.Append(options.HistoryTemplate
                .Replace("${sender}", message.Sender == Sender.AI ? options.AiRoleName : options.UserRoleName)
                .Replace("${content}", message.Content));
                history.Append(",");
            }
        }
        else
        {
            for (int i = 0; i < messages.Count - 2; i += 2)
            {
                history.Append(options.HistoryTemplate
                    .Replace("${user_sender}", options.UserRoleName)
                    .Replace("${user_content}", messages[i].Content)
                    .Replace("${ai_sender}", options.AiRoleName)
                    .Replace("${ai_content}", messages[i + 1].Content));
                history.Append(",");
            }
        }
        if (history.Length > 0)
            history.Remove(history.Length - 1, 1);

        string requestBody = options.Body
            .Replace("${history}", history.ToString())
            .Replace("${prompt}", messages.LastOrDefault().Content);
        
        return requestBody;
    }
}

