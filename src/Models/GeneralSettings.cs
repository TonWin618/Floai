using System;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace Floai.Models;
public class GeneralSettings : INotifyPropertyChanged
{
    private string apiClientName { get; set; }
    public string ApiClientName
    {
        get { return apiClientName; }
        set
        {
            apiClientName = value;
            SettingsPropertyChanged(nameof(ApiClientName));
        }
    }

    private bool startWithWindows;
    public bool StartWithWindows
    {
        get { return startWithWindows; }
        set
        {
            startWithWindows = value;
            SettingsPropertyChanged(nameof(StartWithWindows));
        }
    }

    private double initialPositionX;
    public double InitialPositionX
    {
        get { return initialPositionX; }
        set
        {
            initialPositionX = value;
            SettingsPropertyChanged(nameof(InitialPositionX));
        }
    }

    private double initialPositionY;
    public double InitialPositionY
    {
        get { return initialPositionY; }
        set
        {
            initialPositionY = value;
            SettingsPropertyChanged(nameof(InitialPositionY));
        }
    }

    private double initialWindowHeight;
    public double InitialWindowHeight
    {
        get { return initialWindowHeight; }
        set
        {
            initialWindowHeight = value;
            SettingsPropertyChanged(nameof(InitialWindowHeight));
        }
    }

    private double initialWindowWidth;
    public double InitialWindowWidth
    {
        get { return initialWindowWidth; }
        set
        {
            initialWindowWidth = value;
            SettingsPropertyChanged(nameof(InitialWindowWidth));
        }
    }

    private string messageSaveDirectory;
    public string MessageSaveDirectory
    {
        get { return messageSaveDirectory; }
        set
        {
            messageSaveDirectory = value;
            SettingsPropertyChanged(nameof(MessageSaveDirectory));
        }
    }

    private string themeMode;
    public string ThemeMode
    {
        get { return themeMode; }
        set
        {
            themeMode = value;
            SettingsPropertyChanged(nameof(ThemeMode));
        }
    }

    private string theme;
    public string Theme
    {
        get { return theme; }
        set
        {
            theme = value;
            SettingsPropertyChanged(nameof(Theme));
        }
    }

    private bool isMarkdownEnabled;

    public bool IsMarkdownEnabled
    {
        get { return isMarkdownEnabled; }
        set
        {
            isMarkdownEnabled = value;
            SettingsPropertyChanged(nameof(IsMarkdownEnabled));
        }
    }
    [JsonIgnore]
    public bool isIinitialized = false;

    public event PropertyChangedEventHandler? PropertyChanged = delegate { };

    private void SettingsPropertyChanged(string name)
    {
        if(!isIinitialized)
            return;
        PropertyChanged(this, new PropertyChangedEventArgs(name));
    }
}
