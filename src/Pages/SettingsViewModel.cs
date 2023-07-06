using Floai.ApiClients.abs;
using Floai.Models;
using Floai.Utils.Model;
using Floai.Utils.View;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace Floai.Pages;

public class SettingsViewModel : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged = delegate { };
    private readonly GeneralSettings generalSettings;
    private SettingsManager settingsManager;
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
    private string apiClientOptionsContent;
    public string ApiClientOptionsContent { 
        get { return apiClientOptionsContent; }
        set 
        { 
            apiClientOptionsContent = value;
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(ApiClientOptionsContent)));
        }
    }
    public List<BaseApiClientOptions> optionses { get; set; }

    public SettingsViewModel(GeneralSettings generalSettings, SettingsManager settingsManager, IEnumerable<BaseApiClientOptions> optionses)
    {
        this.settingsManager = settingsManager;
        this.generalSettings = generalSettings;
        this.optionses = optionses.ToList();

        ApiClientNames = new(optionses.Select(o => o.GetType().Name.Replace("ApiClientOptions","")));
        SelectedApiClientName = generalSettings.ApiClientName;

        StartWithWindows = generalSettings.StartWithWindows;
        MessageSaveDirectory = generalSettings.MessageSaveDirectory;
        isMarkdownEnabled = generalSettings.IsMarkdownEnabled;
        generalSettings.PropertyChanged += ConfigAutoStart;
    }

    public void ReadApiClientOptions()
    {
        ApiClientOptionsContent = settingsManager.ReadApiClientOptionsNode(optionses.FirstOrDefault(o=>o.GetType().Name.Replace("ApiClientOptions", "") == SelectedApiClientName), SelectedApiClientName);
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
                generalSettings.StartWithWindows = value;
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
                generalSettings.MessageSaveDirectory = value;
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
                generalSettings.IsMarkdownEnabled = value;
            }
        }
    }

    private void ConfigAutoStart(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(generalSettings.StartWithWindows))
        {
            if (generalSettings.StartWithWindows)
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
