﻿using Floai.Model;
using Floai.Utils;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Floai.Pages;

public partial class ChatView : Window
{
    private ChatViewModel chatViewModel = new();

    private ChatMessageManager messageManager;
    private ChatTopicManager topicManager;

    private static FloatView? floatView;
    private OpenAIClient apiClient;

    private bool isNewTopic = false;

    //Binding ListBox ListSource
    public ObservableCollection<ChatMessage> Messages { get; set; }
    public ObservableCollection<ChatTopic> Topics { get; set; }

    public ChatView()
    {
        InitializeComponent();
        TransparentClick.Enable(this);
        LoadTopics();
        SwitchToLatestTopic();
    }

    private void LoadTopics()
    {
        string messageSaveDictionary = chatViewModel.GetMsgSaveDir();
        topicManager = new ChatTopicManager(messageSaveDictionary);
        List<ChatTopic> topicList = topicManager.GetChatTopics();
        Topics = new ObservableCollection<ChatTopic>(topicList);
        this.TopicCombo.ItemsSource= Topics;
        this.TopicCombo.DisplayMemberPath = "Name";
    }

    private void LoadMessages(ChatTopic topic)
    {
        messageManager = new ChatMessageManager(topic.FilePath);
        List<ChatMessage> messagesList = messageManager.LoadMessages();
        Messages = new ObservableCollection<ChatMessage>(messagesList);
        this.MessageList.ItemsSource = Messages;
        ScrollToBottom();
    }

    private bool InitializeApiClient()
    {
        string? apiKey = chatViewModel.GetApiKey();
        if (string.IsNullOrEmpty(apiKey))
        {
            InputBox.Text = "ApiKey is not configured.";
            ShowSettingsView();
            return false;
        }

        try
        {
            apiClient = new(apiKey);
            return true;
        }
        catch(Exception e)
        {
            InputBox.Text = "Invalid API key.";
            ShowSettingsView();
            return false;
        }
    }

    private async void BtnSend_Click(object sender, RoutedEventArgs e)
    {
        //Ensure proper initialization of apiClient.
        if (!InitializeApiClient())
            return;

        //Generate message sent by the user
        var userMsg = new ChatMessage( DateTime.Now, "user", InputBox.Text);
        if (isNewTopic)
        {
            CreateNewTopic(userMsg.Content);
        }
        Messages.Add(userMsg);
        messageManager.SaveMessage(userMsg);
        InputBox.Text = "";

        //Generate messages sent by the AI
        var newMsg = new ChatMessage( DateTime.Now, "ai", "");
        Messages.Add(newMsg);
        ScrollToBottom();

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
                    ScrollToBottom();
                }
            }
        }
        catch (Exception ex)
        {
            newMsg.AppendContent(ex.Message);
            ScrollToBottom();
        }
        messageManager.SaveMessage(newMsg);
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        if (floatView == null)
        {
            floatView = new FloatView();
        }
        floatView.Left = this.Left + this.Width - floatView.Width;
        floatView.Top = this.Top + this.Height - floatView.Height;
        floatView.Closed += (s, evenArgs) => floatView = null;
        this.Visibility = Visibility.Collapsed;
        floatView.Visibility = Visibility.Visible;
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key == Key.Enter && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
            e.Handled = true; 
            BtnSend?.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }

    private void BtnDrag_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private void BtnSetting_Click(object sender, RoutedEventArgs e)
    {
        ShowSettingsView();
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        chatViewModel.SetWindowSize(this.Width, this.Height);
    }

    private void ShowSettingsView()
    {
        if (WindowHelper.IsWindowOpen<SettingsView>())
            return;
        var settingsView = new SettingsView();
        settingsView.Show();
    }

    private void ScrollToBottom()
    {
        if (MessageList.Items.Count > 0)
        {
            var lastItem = MessageList.Items[MessageList.Items.Count - 1];
            MessageList.ScrollIntoView(lastItem);
        }
    }

    private void CreateNewTopic(string firstMsg)
    {
        var newTopic = topicManager.CreateChatTopic(firstMsg);
        Topics.Add(newTopic);
        SwitchToLatestTopic();
        LoadMessages(newTopic);
        isNewTopic = false;
    }

    private void SwitchToLatestTopic()
    {
        if (Topics!.Count > 0)
        {
            LoadMessages(Topics.Last());
            TopicCombo.SelectedItem = Topics.Last();
        }
    }

    private void TopicCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        LoadMessages((ChatTopic)TopicCombo.SelectedItem);
    }

    private void BtnNewChat_Click(object sender, RoutedEventArgs e)
    {
        MessageList.ItemsSource = null;
        isNewTopic = true;
    }
}
