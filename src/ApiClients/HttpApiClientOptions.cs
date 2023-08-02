using Floai.ApiClients.abs;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json;

namespace Floai.ApiClients
{
    public class HttpApiClientOptions:BaseApiClientOptions
    {
        public string Url { get; set; }

        public Dictionary<string, string>? Headers { get; set; }

        public Dictionary<string, string>? Params { get; set; }

        public string Body { get; set; }
    }
}
