using Floai.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Floai.ApiClients.abs
{
    public abstract class BaseApiClient
    {
        protected BaseApiClient(BaseApiClientOptions options)
        {

        }

        public virtual async Task CreateCompletionAsync(List<ChatMessage> messages, Action<string> onDataReceived)
        {
            return;
        }
    }
}
