using Floai.Model;
using Floai.Models;
using Floai.Utils.Data;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Floai.Pages
{
    public class ChatViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged = delegate { };
        public Action ScrollToBottom;//temp

        private OpenAIClient apiClient;
        private List<string> apiKeys;
        private int lastApiKeyIndex;
        private ChatMessageManager messageManager;
        private ChatTopicManager topicManager;
        public FileWatcher fileWatcher;
        private readonly AppSettings appSettings;

        private string inputContent;
        public string InputContent
        {
            get { return inputContent; }
            set
            {
                inputContent = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(InputContent)));
            }
        }

        public ObservableCollection<ChatMessage> Messages { get; set; }
        public ObservableCollection<ChatTopic> Topics { get; set; }

        private ChatMessage selectedMessageItem;
        public ChatMessage SelectedMessageItem
        {
            get
            {
                return selectedMessageItem;
            }
            set
            {
                selectedMessageItem = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedMessageItem)));
            }
        }
        private ChatTopic? selectedTopicItem;
        public ChatTopic? SelectedTopicItem
        {
            get
            {
                return selectedTopicItem;
            }
            set
            {
                selectedTopicItem = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedTopicItem)));
            }
        }

        private bool isNewTopic = false;
        public ChatViewModel(Action ScrollToBottom, AppSettings appSettings)
        {
            this.appSettings = appSettings;
            this.ScrollToBottom += ScrollToBottom;
            Messages = new ObservableCollection<ChatMessage>();
            Topics = new ObservableCollection<ChatTopic>();
            //appSettings.SettingChanged += OnSettingChange;
            ReloadData();
        }

        private void OnSettingChange(string key)
        {
            if (key == "messageSaveDirectory")
            {
                ReloadData();
                fileWatcher = new(appSettings.MessageSaveDirectory, OnMsgLogFileChanged);
            }
            if (key == "apiKeys/apiKey")
            {
                ReloadApiKeys();
            }
        }

        private void ReloadData()
        {
            ReloadTopics();
            SwitchToLatestTopic();
        }

        public void OnMsgLogFileChanged(object sender, FileSystemEventArgs e)
        {
            //Console.WriteLine("File {0} was created.", e.Name);
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                if (e.Name != Path.GetFileName(Topics.Last().FilePath))
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        ReloadData();
                    }));
                }
            }
            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    ReloadData();
                }));
            }
        }

        private void ReloadTopics()
        {
            string messageSaveDictionary = appSettings.MessageSaveDirectory;
            topicManager = new ChatTopicManager(messageSaveDictionary);
            Topics.Clear();
            selectedTopicItem = null;
            topicManager.GetChatTopics().ForEach(Topics.Add);
            if (Topics.Count == 0)
            {
                isNewTopic = true;
            }
            apiKeys = appSettings.ApiKeys;
        }

        //To create a new topic, the first message needs to name the topic.
        public void CreateNewTopic(string firstMsg)
        {
            var newTopic = topicManager.CreateChatTopic(firstMsg);
            Topics.Add(newTopic);
            SwitchToLatestTopic();
            SelectedTopicItem = newTopic;
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
                SelectedTopicItem = Topics.Last();
                SwitchTopic();
            }
        }

        public void SwitchTopic()
        {
            if (SelectedTopicItem == null)
                return;
            if (isNewTopic) isNewTopic = false;
            messageManager = new ChatMessageManager(SelectedTopicItem.FilePath);
            Messages.Clear();
            messageManager.LoadMessages().ForEach(Messages.Add);
            ScrollToBottom();//temp
        }
        public (double, double) ReadWindowSize()
        {
            double windowWidth = appSettings.InitialWindowWidth;
            double windowHeight = appSettings.InitialWindowWidth;
            return (windowWidth, windowHeight);
        }

        public void WriteWindowSize(double width, double height)
        {
            appSettings.InitialWindowWidth = width;
            appSettings.InitialWindowHeight = height;
        }

        public void InitializeApiClient()
        {
            if (apiKeys.Count == 0)
            {
                throw new Exception("API key not configured.");
            }
            string apiKey = apiKeys[lastApiKeyIndex];
            apiClient = new(apiKey);

            lastApiKeyIndex++;
            if (lastApiKeyIndex >= apiKeys.Count)
                lastApiKeyIndex = 0;
        }

        public void ReloadApiKeys()
        {
            //If the list of ApiKeys changes in the configuration, then reloading is necessary.
            apiKeys = appSettings.ApiKeys;
            lastApiKeyIndex = 0;
        }

        public List<Message> GenerateChatContext()
        {
            //Generate message sent by the user
            var userMsg = new ChatMessage(DateTime.Now, Sender.User, InputContent);
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
                msg => new Message(msg.Sender == Sender.User ? Role.User : Role.Assistant, msg.Content))
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
            var newMsg = new ChatMessage(DateTime.Now, Sender.AI, "");
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
                messageManager.SaveMessage(newMsg);
            }
            catch (Exception ex)
            {
                newMsg.AppendContent(ex.Message);
                ScrollToBottom();//temp
            }
        }
    }
}
