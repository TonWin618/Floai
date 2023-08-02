using Floai.ApiClients.abs;
using Floai.Model;
using Floai.Models;
using Floai.Utils.Model;
using System;
using System.Collections;
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
        public Action ScrollToBottom = delegate { };// TODO: Remove it

        private BaseApiClient apiClient;

        private ChatMessageManager messageManager;
        private ChatTopicManager topicManager;
        public FileWatcher fileWatcher;

        private readonly GeneralSettings generalSettings;

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





        public ChatViewModel(GeneralSettings generalSettings, BaseApiClient apiClient)
        {
            this.apiClient = apiClient;
            this.generalSettings = generalSettings;
            Messages = new ObservableCollection<ChatMessage>();
            Topics = new ObservableCollection<ChatTopic>();
            generalSettings.PropertyChanged += OnSettingsChanged;
            ReloadViewModel();
        }

        private void OnSettingsChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(generalSettings.MessageSaveDirectory))
            {
                ReloadViewModel();
                fileWatcher = new(generalSettings.MessageSaveDirectory, OnMessageSavePathChanged);
            }
            if(e.PropertyName == nameof(generalSettings.IsMarkdownEnabled))
            {
                OnTopicSwitched();
            }
        }

        public void OnMessageSavePathChanged(object sender, FileSystemEventArgs e)
        {
            //Console.WriteLine("File {0} was created.", e.Name);
            if (e.ChangeType == WatcherChangeTypes.Created)
            {
                if (e.Name != Path.GetFileName(Topics.Last().FilePath))
                {
                    System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                    {
                        ReloadViewModel();
                    }));
                }
            }
            if (e.ChangeType == WatcherChangeTypes.Deleted)
            {
                System.Windows.Application.Current.Dispatcher.Invoke((Action)(() =>
                {
                    ReloadViewModel();
                }));
            }
        }

        private void ReloadViewModel()
        {
            ReloadTopics();
            SwitchToLatestTopic();
        }
        

        private void ReloadTopics()
        {
            string messageSaveDictionary = generalSettings.MessageSaveDirectory;
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

        // Before the new topic is confirmed to be created.
        public void PreCreateNewTopic()
        {
            Messages.Clear();
            isNewTopic = true;
        }

        private void SwitchToLatestTopic()
        {
            if (Topics.Count > 0)
            {
                SelectedTopicItem = Topics.Last();
                OnTopicSwitched();
            }
        }

        public void OnTopicSwitched()
        {
            if (SelectedTopicItem == null)
                return;
            // Cancel the pre-creation of the new topic.
            if (isNewTopic) isNewTopic = false;

            messageManager = new ChatMessageManager(SelectedTopicItem.FilePath);
            Messages.Clear();
            messageManager.LoadMessages().ForEach(Messages.Add);
            ScrollToBottom();// TODO: Remove it
        }
        public (double, double) ReadWindowSize()
        {
            double windowWidth = generalSettings.InitialWindowWidth;
            double windowHeight = generalSettings.InitialWindowWidth;
            return (windowWidth, windowHeight);
        }

        public void WriteWindowSize(double width, double height)
        {
            generalSettings.InitialWindowWidth = width;
            generalSettings.InitialWindowHeight = height;
        }

        public async Task RequestAndReceiveResponse()
        {
            // Generate message sent by the user
            var userMsg = new ChatMessage(DateTime.Now, Sender.User, InputContent);
            InputContent = "";

            if (isNewTopic)
            {
                CreateNewTopic(userMsg.Content);
            }

            Messages.Add(userMsg);
            ScrollToBottom();//temp
            messageManager.SaveMessage(userMsg);

            // Generate messages sent by the AI
            var newMsg = new ChatMessage(DateTime.Now, Sender.AI, "");
            Messages.Add(newMsg);
            ScrollToBottom();// TODO: Remove it

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
