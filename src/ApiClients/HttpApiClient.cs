using Floai.ApiClients.abs;
using Floai.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Floai.ApiClients
{
    public class HttpApiClient : BaseApiClient
    {
        private readonly HttpApiClientOptions options;
        public HttpApiClient(BaseApiClientOptions options) : base(options)
        {
            this.options = options as HttpApiClientOptions ?? throw new ArgumentException();
        }

        public override Task CreateCompletionAsync(List<ChatMessage> messages, Action<string, bool> onDataReceived)
        {
            return Task.CompletedTask;
        }
    }
}
