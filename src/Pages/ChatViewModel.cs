﻿using Floai.ApiClients.abs;
using Floai.Model;
using Floai.Models;
using Floai.Utils.Model;
using System;
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

        private BaseApiClient apiClient;

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





        public ChatViewModel(Action ScrollToBottom, AppSettings appSettings, BaseApiClient apiClient)
        {
            this.apiClient = apiClient;
            this.appSettings = appSettings;
            this.ScrollToBottom += ScrollToBottom;
            Messages = new ObservableCollection<ChatMessage>();
            Topics = new ObservableCollection<ChatTopic>();
            appSettings.SettingChanged += OnSettingChange;
            ReloadData();
        }

        private void OnSettingChange(string key)
        {
            if (key == nameof(appSettings.MessageSaveDirectory))
            {
                ReloadData();
                fileWatcher = new(appSettings.MessageSaveDirectory, OnMsgLogFileChanged);
            }
            if(key == nameof(appSettings.IsMarkdownEnabled))
            {
                SwitchTopic();
            }
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

        private void ReloadData()
        {
            ReloadTopics();
            SwitchToLatestTopic();
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

        public async Task RequestAndReceiveResponse()
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

            //Generate messages sent by the AI
            var newMsg = new ChatMessage(DateTime.Now, Sender.AI, "");
            Messages.Add(newMsg);
            ScrollToBottom();//temp

            bool saveMsg = true;

            await apiClient.CreateCompletionAsync(Messages.SkipLast(1).ToList(), (text, saved) =>
            {
                newMsg.AppendContent(text);
                if(saved == false) saveMsg = false;
                ScrollToBottom();
            });

            if (saveMsg) messageManager.SaveMessage(newMsg);

            return;
        }
    }
}
