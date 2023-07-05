using Floai.Models;
using Floai.Utils.App;
using Floai.Utils.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Floai.Pages;

public class SettingsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged = delegate { };
    private readonly AppSettings appSettings;

    public ObservableCollection<string> ApiKeys { get; set; }

    public SettingsViewModel(AppSettings appSettings)
    {
        this.appSettings = appSettings;
        ApiKeys = new ObservableCollection<string>(appSettings.ApiKeys);
        StartWithWindows = appSettings.StartWithWindows;
        MessageSaveDirectory = appSettings.MessageSaveDirectory;
        isMarkdownEnabled = appSettings.IsMarkdownEnabled;
        appSettings.SettingChanged += ConfigAutoStart;
    }

    public void AppendApiKey(string apiKey)
    {
        string pattern = @"[^a-zA-Z0-9-]";
        Regex.Replace(apiKey, pattern, "");
        if (string.IsNullOrEmpty(apiKey))
            return;
        if (!ApiKeys.Contains(apiKey))
        {
            this.ApiKeys.Add(apiKey);
            appSettings.ApiKeys.Add(apiKey);
            //AppConfiger.SetValue("isApiKeysReloadNeeded", "True");
        }
    }

    public void RemoveApiKey(string apiKey)
    {
        this.ApiKeys.Remove(apiKey);
        appSettings.ApiKeys.Remove(apiKey);
        //AppConfiger.SetValue("isApiKeysReloadNeeded", "True");
    }

    private bool startWithWindows;
    public bool StartWithWindows
    {
        get { return startWithWindows; }
        set
        {
            if (startWithWindows != value)
            {
                startWithWindows = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(StartWithWindows)));
                appSettings.StartWithWindows = value;
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
                appSettings.MessageSaveDirectory = value;
            }
        }
    }

    private bool isMarkdownEnabled;
    public bool IsMarkdownEnabled
    {
        get { return isMarkdownEnabled; }
        set
        {
            if (isMarkdownEnabled != value)
            {
                isMarkdownEnabled = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(IsMarkdownEnabled)));
                appSettings.IsMarkdownEnabled = value;
            }
        }
    }

    private void ConfigAutoStart(string key)
    {
        if (key == nameof(appSettings.StartWithWindows))
        {
            if (appSettings.StartWithWindows)
            {
                AppAutoStarter.EnableAutoStart();
            }
            else
            {
                AppAutoStarter.DisableAutoStart();
            }
        }
    }
}
