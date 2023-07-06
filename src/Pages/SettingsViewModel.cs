using Floai.Models;
using Floai.Utils.Client;
using Floai.Utils.View;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Floai.Pages;

public class SettingsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged = delegate { };
    private readonly AppSettings appSettings;
    private ApiClientFinder finder = new("Floai.ApiClients");
    public string ErrorMessage { get; set; }
    public ObservableCollection<string> ApiClientNames { get; set; }

    private string selectedApiClientName;
    public string SelectedApiClientName
    {
        get { return selectedApiClientName; }
        set
        {
            if (selectedApiClientName != value)
            {
                selectedApiClientName = value;
                PropertyChanged(this, new PropertyChangedEventArgs(nameof(SelectedApiClientName)));
            }
        }
    }

    public SettingsViewModel(AppSettings appSettings)
    {
        this.appSettings = appSettings;
        ApiClientNames = new();
        foreach (var item in finder.GetApiClientClasses().Select(c => c.Name))
        {
            ApiClientNames.Add(item);
        }
        SelectedApiClientName = ApiClientNames.First();
        StartWithWindows = appSettings.StartWithWindows;
        MessageSaveDirectory = appSettings.MessageSaveDirectory;
        isMarkdownEnabled = appSettings.IsMarkdownEnabled;
        appSettings.SettingChanged += ConfigAutoStart;
    }

    public void LoadApiClientOptions()
    {

    }

    public void SaveApiClientOptions()
    {
        
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
