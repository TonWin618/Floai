using Floai.Model;
using Floai.Utils;
using OpenAI;
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
        public OpenAIClient apiClient;
        public ChatMessageManager messageManager;
        private ChatMessage CurMessageItem { get; set; }
        private ChatTopicManager topicManager;
        private ChatTopic CurTopicItem { get; set; }
        public ObservableCollection<ChatMessage> Messages { get; set; }
        public ObservableCollection<ChatTopic> Topics { get; set; }

        public bool isNewTopic = false;
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
            //this.TopicCombo.ItemsSource= Topics;
            //this.TopicCombo.DisplayMemberPath = "Name";
        }

        public void CreateNewTopic(string firstMsg)
        {
            var newTopic = topicManager.CreateChatTopic(firstMsg);
            Topics.Add(newTopic);
            SwitchToLatestTopic();
            LoadMessages(newTopic);
            //ScrollToBottom();
            isNewTopic = false;
        }

        private void SwitchToLatestTopic()
        {
            if (Topics.Count > 0)
            {
                LoadMessages(Topics.Last());
                //ScrollToBottom();
                CurTopicItem = Topics.Last();
            }
        }

        public void LoadMessages(ChatTopic topic)
        {
            messageManager = new ChatMessageManager(topic.FilePath);
            Messages.Clear();
            messageManager.LoadMessages().ForEach(Messages.Add);
            //Messages = new ObservableCollection<ChatMessage>(messagesList);
            //this.MessageList.ItemsSource = Messages;
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

        public bool InitializeApiClient()
        {
            string? apiKey = GetApiKey();
            if (string.IsNullOrEmpty(apiKey))
            {
                //InputBox.Text = "ApiKey is not configured.";
                //ShowSettingsView();
                return false;
            }

            try
            {
                apiClient = new(apiKey);
                return true;
            }
            catch (Exception e)
            {
                //InputBox.Text = "Invalid API key.";
                //ShowSettingsView();
                return false;
            }
        }
    }
}
