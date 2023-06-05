﻿using Floai.Utils.App;
using Floai.Utils.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Floai.Pages;

public class SettingsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged = delegate { };

    public ObservableCollection<string> ApiKeys { get; set; }
    public enum ChatBubbleLayout
    {
        Left,
        Right,
        Alternate
    }

    public SettingsViewModel()
    {
        ApiKeys = new ObservableCollection<string>(AppConfiger.GetValue("apiKeys").Split(";"));
        StartWithWindows = AppConfiger.GetValue<bool>("startWithWindows");
        MessageSaveDirectory = AppConfiger.GetValue("messageSaveDirectory");
        if (Enum.TryParse(AppConfiger.GetValue("chatBubbleLayout"), out ChatBubbleLayout bubbleLayout))
        {
            BubbleLayout = bubbleLayout;
        }
    }

    public void AppendApiKey(string apiKey)
    {
        if (!ApiKeys.Contains(apiKey))
        {
            this.ApiKeys.Add(apiKey);
        }
        AppConfiger.SetValue("apiKeys", string.Join(";", ApiKeys));
    }

    public void RemoveApiKey(string apiKey)
    {
        this.ApiKeys.Remove(apiKey);
        AppConfiger.SetValue("apiKeys", string.Join(";", ApiKeys));
    }

    private bool _startWithWindows;
    public bool StartWithWindows
    {
        get { return _startWithWindows; }
        set
        {
            if (_startWithWindows != value)
            {
                if (value)
                {
                    AppAutoStarter.EnableAutoStart();
                }
                else
                {
                    AppAutoStarter.DisableAutoStart();
                }
                _startWithWindows = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(StartWithWindows)));
                AppConfiger.SetValue("startWithWindows", value.ToString());
            }
        }
    }

    private string _messageSaveDirectory;
    public string MessageSaveDirectory
    {
        get { return _messageSaveDirectory; }
        set
        {
            if (_messageSaveDirectory != value)
            {
                _messageSaveDirectory = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(MessageSaveDirectory)));
                AppConfiger.SetValue("messageSaveDirectory", value);
            }
        }
    }

    private ChatBubbleLayout _bubbleLayout;
    public ChatBubbleLayout BubbleLayout
    {
        get { return _bubbleLayout; }
        set
        {
            if (_bubbleLayout != value)
            {
                _bubbleLayout = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(BubbleLayout)));
                AppConfiger.SetValue("chatBubbleLayout", value.ToString());
            }
        }
    }
}
