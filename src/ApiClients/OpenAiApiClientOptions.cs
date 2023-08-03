using System.Collections.Generic;
using Floai.ApiClients.abs;

namespace Floai.ApiClients
{
    public class OpenAiApiClientOptions : BaseApiClientOptions
    {
        public List<string> ApiKeys { get; set; }
    }
}
