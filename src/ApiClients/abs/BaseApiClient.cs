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

        /// <summary>
        /// Creates a completion asynchronously based on the provided list of chat messages.
        /// </summary>
        /// <param name="messages">The list of chat messages.</param>
        /// <param name="onDataReceived">Callback function to be executed when data is received.
        ///     - string: The content to append to the message.
        ///     - bool: Indicates whether the message should be saved or not.
        ///             If the value passed in during a single call is false, the message will not be saved.
        /// </param>
        /// <returns>A task representing the asynchronous operation.</returns>
        public virtual async Task CreateCompletionAsync(List<ChatMessage> messages, Action<string,bool> onDataReceived)
        {
            return;
        }
    }
}
