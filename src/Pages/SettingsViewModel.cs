using Floai.Utils.App;
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
        ApiKeys = new ObservableCollection<string>(AppConfiger.GetValues("apiKeys/apiKey"));
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
            AppConfiger.AddValue("apiKeys/apiKey", apiKey);
            AppConfiger.SetValue("isApiKeysReloadNeeded", "True");
        }
        
    }

    public void RemoveApiKey(string apiKey)
    {
        this.ApiKeys.Remove(apiKey);
        AppConfiger.RemoveValue("apiKeys/apiKey", apiKey);
        AppConfiger.SetValue("isApiKeysReloadNeeded", "True");
    }

    private bool startWithWindows;
    public bool StartWithWindows
    {
        get { return startWithWindows; }
        set
        {
            if (startWithWindows != value)
            {
                if (value)
                {
                    AppAutoStarter.EnableAutoStart();
                }
                else
                {
                    AppAutoStarter.DisableAutoStart();
                }
                startWithWindows = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(StartWithWindows)));
                AppConfiger.SetValue("startWithWindows", value.ToString());
            }
        }
    }

    private string messageSaveDirectory;
    public string MessageSaveDirectory
    {
        get { return messageSaveDirectory; }
        set
        {
            if (messageSaveDirectory != value)
            {
                messageSaveDirectory = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(MessageSaveDirectory)));
                AppConfiger.SetValue("messageSaveDirectory", value);
            }
        }
    }

    private ChatBubbleLayout bubbleLayout;
    public ChatBubbleLayout BubbleLayout
    {
        get { return bubbleLayout; }
        set
        {
            if (bubbleLayout != value)
            {
                bubbleLayout = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(BubbleLayout)));
                AppConfiger.SetValue("chatBubbleLayout", value.ToString());
            }
        }
    }
}
