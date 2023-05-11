using Floai.Model;
using Floai.Utils;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;

namespace Floai.Pages
{
    public class ChatViewModel
    {
        private OpenAIClient apiClient;
        private ChatMessageManager messageManager;
        private ChatTopicManager topicManager;
        public string InputContent { get; set; }
        public ChatMessage CurMessageItem { get; set; }
        public ChatTopic CurTopicItem { get; set; }
        public ObservableCollection<ChatMessage> Messages { get; set; }
        public ObservableCollection<ChatTopic> Topics { get; set; }

        private bool isNewTopic = false;

        public ChatViewModel()
        {
            Messages = new ObservableCollection<ChatMessage>();
            Topics = new ObservableCollection<ChatTopic>();
            LoadTopics();
            SwitchToLatestTopic();
        }

        private void LoadTopics()
        {
            string messageSaveDictionary = GetMsgSaveDir();
            topicManager = new ChatTopicManager(messageSaveDictionary);
            Topics.Clear();
            topicManager.GetChatTopics().ForEach(Topics.Add);
        }

        public void CreateNewTopic(string firstMsg)
        {
            var newTopic = topicManager.CreateChatTopic(firstMsg);
            Topics.Add(newTopic);
            SwitchToLatestTopic();
            LoadMessages();
            isNewTopic = false;
        }

        public void BeforeCreateNewTopic()
        {
            Messages.Clear();
            isNewTopic = true;
        }

        private void SwitchToLatestTopic()
        {
            if (Topics.Count > 0)
            {
                CurTopicItem = Topics.Last();
                LoadMessages();
            }
        }

        public void LoadMessages()
        {
            messageManager = new ChatMessageManager(CurTopicItem.FilePath);
            Messages.Clear();
            messageManager.LoadMessages().ForEach(Messages.Add);
        }

        public void SetWindowSize(double width, double height)
        {
            AppConfiger.SetValue("initialWindowHeight", width.ToString());
            AppConfiger.SetValue("initialWindowWidth", height.ToString());
        }
        public string GetMsgSaveDir()
        {
            return AppConfiger.GetValue("messageSaveDirectory");
        }

        public string GetApiKey()
        {
            return AppConfiger.GetValue("apiKey");
        }

        public void InitializeApiClient()
        {
            string? apiKey = GetApiKey();
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("ApiKey is not configured.");
            }
            apiClient = new(apiKey);
        }

        public async void SendMessage()
        {
            try
            {
                InitializeApiClient();
            }
            catch (Exception ex)
            {
                InputContent = ex.Message;
                return;
            }

            //Generate message sent by the user
            var userMsg = new ChatMessage(DateTime.Now, "user", InputContent);
            InputContent = "";
            if (isNewTopic)
            {
                CreateNewTopic(userMsg.Content);
            }
            Messages.Add(userMsg);
            messageManager.SaveMessage(userMsg);

            //Generate messages sent by the AI
            var newMsg = new ChatMessage(DateTime.Now, "ai", "");
            Messages.Add(newMsg);

            //Context of conversations between user and AI.
            var messageContext = new List<Message> { };

            foreach (var message in Messages)
                messageContext.Add(new Message(message.Sender == "user" ? Role.User : Role.Assistant, message.Content));

            messageContext.Add(new Message(Role.User, userMsg.Content));

            var chatRequest = new ChatRequest(messageContext, OpenAI.Models.Model.GPT3_5_Turbo);
            try
            {
                await foreach (var result in apiClient.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest))
                {
                    foreach (var choice in result.Choices.Where(choice => choice.Delta?.Content != null))
                    {
                        newMsg.AppendContent(choice.Delta.Content);
                    }
                }
            }
            catch (Exception ex)
            {
                newMsg.AppendContent(ex.Message);
            }
            messageManager.SaveMessage(newMsg);
        }
    }
}
