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
    public ChatViewModel chatViewModel;
    private static FloatView? floatView;
    public ChatView()
    {
        InitializeComponent();
        TransparentClick.Enable(this);
        chatViewModel = new ChatViewModel();
        this.DataContext = chatViewModel;
    }
    private async void BtnSend_Click(object sender, RoutedEventArgs e)
    {
        chatViewModel.SendMessage();
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
    

    private void TopicCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        chatViewModel.LoadMessages();
        ScrollToBottom();
    }

    private void BtnNewChat_Click(object sender, RoutedEventArgs e)
    {
        chatViewModel.BeforeCreateNewTopic();
    }
}
