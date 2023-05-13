using Floai.Model;
using Floai.Utils;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Floai.Pages
{
    public class ChatViewModel: INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        public Action ScrollToBottom;//temp

        private OpenAIClient apiClient;
        private ChatMessageManager messageManager;
        private ChatTopicManager topicManager;

        private string inputContent;
        public string InputContent
        {
            get{ return inputContent; }
            set
            {
                inputContent = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(InputContent)));
            }
        }

        public ObservableCollection<ChatMessage> Messages { get; set; }
        public ObservableCollection<ChatTopic> Topics { get; set; }

        public ChatMessage CurMessageItem { get; set; }
        public ChatTopic CurTopicItem { get; set; }
        private bool isNewTopic = false;
        public ChatViewModel(Action ScrollToBottom)
        {
            this.ScrollToBottom += ScrollToBottom;
            Messages = new ObservableCollection<ChatMessage>();
            Topics = new ObservableCollection<ChatTopic>();
            LoadTopics();
            SwitchToLatestTopic();
        }

        private void LoadTopics()
        {
            string messageSaveDictionary = AppConfiger.GetValue("messageSaveDirectory");
            topicManager = new ChatTopicManager(messageSaveDictionary);
            Topics.Clear();
            topicManager.GetChatTopics().ForEach(Topics.Add);
            if(Topics.Count == 0)
            {
                isNewTopic= true;
            }
        }

        //To create a new topic, the first message needs to name the topic.
        public void CreateNewTopic(string firstMsg)
        {
            var newTopic = topicManager.CreateChatTopic(firstMsg);
            Topics.Add(newTopic);
            SwitchToLatestTopic();
            CurTopicItem = newTopic;
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
            ScrollToBottom();//temp
        }
        public (double,double) ReadWindowSize()
        {
            double windowWidth = AppConfiger.GetValue<double>("initialWindowWidth");
            double windowHeight = AppConfiger.GetValue<double>("initialWindowHeight");
            return (windowWidth, windowHeight);
        }

        public void WriteWindowSize(double width, double height)
        {
            AppConfiger.SetValue("initialWindowHeight", width.ToString());
            AppConfiger.SetValue("initialWindowWidth", height.ToString());
        }

        public void InitializeApiClient()
        {
            string? apiKey = AppConfiger.GetValue("apiKey"); ;
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new Exception("ApiKey is not configured.");
            }
            apiClient = new(apiKey);
        }

        public List<Message> GenerateChatContext()
        {
            //Generate message sent by the user
            var userMsg = new ChatMessage(DateTime.Now, "user", InputContent);
            InputContent = "";
            if (isNewTopic)
            {
                CreateNewTopic(userMsg.Content);
            }
            Messages.Add(userMsg);
            ScrollToBottom();//temp
            messageManager.SaveMessage(userMsg);

            //Context of conversations between user and AI.
            var messageContext = Messages.Select(
                msg => new Message(msg.Sender=="user"?Role.User:Role.Assistant,msg.Content))
                .ToList();

            messageContext.Add(new Message(Role.User, userMsg.Content));

            return messageContext;
        }

        public async Task RequestAndReceiveResponse()
        {
            //Initialize OpenAI API client.
            try
            {
                InitializeApiClient();
            }
            catch (Exception ex)
            {
                InputContent = ex.Message;
                return;
            }

            List<Message> chatContext = GenerateChatContext();

            //Generate messages sent by the AI
            var newMsg = new ChatMessage(DateTime.Now, "ai", "");
            Messages.Add(newMsg);
            ScrollToBottom();//temp
            var chatRequest = new ChatRequest(chatContext, OpenAI.Models.Model.GPT3_5_Turbo);
            try
            {
                await foreach (var result in apiClient.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest))
                {
                    foreach (var choice in result.Choices.Where(choice => choice.Delta?.Content != null))
                    {
                        newMsg.AppendContent(choice.Delta.Content);
                        ScrollToBottom();//temp
                    }
                }
            }
            catch (Exception ex)
            {
                newMsg.AppendContent(ex.Message);
                ScrollToBottom();//temp
            }
            messageManager.SaveMessage(newMsg);
        }
    }
}
