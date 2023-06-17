﻿using Floai.Utils.App;
using Floai.Utils.Data;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Text.RegularExpressions;

namespace Floai.Pages;

public class SettingsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged = delegate { };

    public ObservableCollection<string> ApiKeys { get; set; }

    public SettingsViewModel()
    {
        ApiKeys = new ObservableCollection<string>(AppConfiger.GetValues("apiKeys/apiKey"));
        StartWithWindows = AppConfiger.GetValue<bool>("startWithWindows");
        MessageSaveDirectory = AppConfiger.GetValue("messageSaveDirectory");
        isMarkdownEnabled = AppConfiger.GetValue<bool>("isMarkdownEnabled");
        AppConfiger.SettingChanged += ConfigAutoStart;
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
            AppConfiger.AddValue("apiKeys/apiKey", apiKey);
            //AppConfiger.SetValue("isApiKeysReloadNeeded", "True");
        }
    }

    public void RemoveApiKey(string apiKey)
    {
        this.ApiKeys.Remove(apiKey);
        AppConfiger.RemoveValue("apiKeys/apiKey", apiKey);
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
                AppConfiger.SetValue("isMarkdownEnabled", value.ToString());
            }
        }
    }

    private void ConfigAutoStart(string key, string value)
    {
        if (key == "startWithWindows")
        {
            if (bool.Parse(value))
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
