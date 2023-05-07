using Floai.Model;
using Floai.Utils;
using OpenAI;
using OpenAI.Chat;
using Stylet;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Floai.Pages;

public partial class ChatView : Window
{
    private readonly AppConfiger configer = new("config.xml");
    private readonly ChatMessageManager messageManager = new("MessageLogging/message.txt");
    private static FloatView? floatView;
    private readonly OpenAIClient api;

    //Binding ListBox ListSource
    public ObservableCollection<ChatMessage> Messages { get; set; }

    public ChatView()
    {
        InitializeComponent();
        TransparentClick.Enable(this);
        api = new(configer.GetValue("apiKey"));
        
        Messages = new ObservableCollection<ChatMessage>();
        
        foreach (var msg in messageManager.LoadMessages())
        {
            Messages.Add(msg);
        }
        this.MessageList.ItemsSource = Messages;
    }

    private void ScrollToBottom()
    {
        if (MessageList.Items.Count > 0)
        {
            var lastItem = MessageList.Items[MessageList.Items.Count - 1];
            MessageList.ScrollIntoView(lastItem);
        }
    }


    private async void BtnSend_Click(object sender, RoutedEventArgs e)
    {
        //Generate message sent by the user
        var userMsg = new ChatMessage( DateTime.Now, "user", InputBox.Text);
        Messages.Add(userMsg);
        messageManager.SaveMessage(userMsg);
        ScrollToBottom();
        InputBox.Text = "";

        //Generate messages sent by the AI
        var newMsg = new ChatMessage( DateTime.Now, "ai", "");
        Messages.Add(newMsg);

        var messages = new List<Message>
        {
            new Message(Role.User, userMsg.Content),
        };
        var chatRequest = new ChatRequest(messages, OpenAI.Models.Model.GPT3_5_Turbo);
        try
        {
            await foreach (var result in api.ChatEndpoint.StreamCompletionEnumerableAsync(chatRequest))
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

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        configer.SetValue("initialWindowHeight", this.Height.ToString());
        configer.SetValue("initialWindowWidth", this.Width.ToString());
    }
}
