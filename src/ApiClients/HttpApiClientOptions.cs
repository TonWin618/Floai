using Floai.ApiClients.abs;

namespace Floai.ApiClients
{
    public class HttpApiClientOptions:BaseApiClientOptions
    {
        public string Url { get;set; }
        public string Authcode { get;set; }
        public int MaxTokens { get; set; }
    }
}
