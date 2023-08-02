using Floai.ApiClients.abs;
using System.Collections.Generic;

namespace Floai.ApiClients
{
    public class HttpApiClientOptions:BaseApiClientOptions
    {
        public string Url { get; set; }

        public Dictionary<string, string>? Headers { get; set; }

        public Dictionary<string, string>? Params { get; set; }

        public string Body { get; set; }

        public string HistoryFormat { get; set; }
        public string AiRoleName { get; set; }
        public string UserRoleName { get; set; }
    }
}
