using Floai.Utils.Data;
using System;
using System.ComponentModel;

namespace Floai.Pages;

public class SettingsViewModel : INotifyPropertyChanged
{

    public event PropertyChangedEventHandler? PropertyChanged = delegate { };
    public enum ChatBubbleLayout
    {
        Left,
        Right,
        Alternate
    }

    public SettingsViewModel()
    {
        ApiKey = AppConfiger.GetValue("apiKey");
        StartWithWindows = AppConfiger.GetValue<bool>("startWithWindows");
        MessageSaveDirectory = AppConfiger.GetValue("messageSaveDirectory");
        if (Enum.TryParse(AppConfiger.GetValue("chatBubbleLayout"), out ChatBubbleLayout bubbleLayout))
        {
            BubbleLayout = bubbleLayout;
        }
    }

    private string _apiKey;
    public string ApiKey
    {
        get { return _apiKey; }
        set
        {
            if (_apiKey != value)
            {
                _apiKey = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(ApiKey)));
                AppConfiger.SetValue("apiKey", value);
            }
        }
    }

    private bool _startWithWindows;
    public bool StartWithWindows
    {
        get { return _startWithWindows; }
        set
        {
            if (_startWithWindows != value)
            {
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
