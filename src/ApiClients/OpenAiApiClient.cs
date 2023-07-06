using Floai.ApiClients.abs;
using Floai.Model;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Floai.ApiClients
{
    public class OpenAiApiClient : BaseApiClient
    {
        OpenAiApiClientOptions options;

        int lastApiKeyIndex;
        private OpenAIClient client;

        public OpenAiApiClient(BaseApiClientOptions options) : base(options)
        {
            this.options = options as OpenAiApiClientOptions ?? throw new ArgumentException();
        }

        public override async Task CreateCompletionAsync(List<ChatMessage> messages, Action<string, bool> onDataReceived)
        {
            if (options.ApiKeys.Count == 0)
            {
                onDataReceived("API key not configured.",false);
                return;
            }

            string apiKey = options.ApiKeys[lastApiKeyIndex];
            lastApiKeyIndex++;
            if (lastApiKeyIndex >= options.ApiKeys.Count)
                lastApiKeyIndex = 0;

            try
            {
                client = new OpenAIClient(apiKey);
            }
            catch (Exception ex)
            {
                onDataReceived(ex.Message, false);
                return;
            }

            var prompt = GeneratePrompt(messages);
            var chatRequest = new ChatRequest(prompt, OpenAI.Models.Model.GPT3_5_Turbo);
            try
            {
                await foreach (var result in client.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest))
                {
                    foreach (var choice in result.Choices.Where(choice => choice.Delta?.Content != null))
                    {
                        onDataReceived(choice.Delta.Content, true);
                    }
                }
            }
            catch (Exception ex)
            {
                onDataReceived(ex.Message,false);
            }

            return;
        }

        public List<Message> GeneratePrompt(List<ChatMessage> messages)
        {
            //Context of conversations between user and AI.
            var messageContext = messages.Select(
                msg => new Message(msg.Sender == Sender.User ? Role.User : Role.Assistant, msg.Content))
                .ToList();
            return messageContext;
        }
    }
}
