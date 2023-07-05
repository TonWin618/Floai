using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System;

namespace Floai.Models;
public class AppSettings
{
    private List<string> apiKeys;
    public List<string> ApiKeys
    {
        get { return apiKeys; }
        set
        {
            apiKeys = value;
            OnSettingChanged(nameof(ApiKeys));
        }
    }

    private bool startWithWindows;
    public bool StartWithWindows
    {
        get { return startWithWindows; }
        set
        {
            startWithWindows = value;
            OnSettingChanged(nameof(StartWithWindows));
        }
    }

    private double initialPositionX;
    public double InitialPositionX
    {
        get { return initialPositionX; }
        set
        {
            initialPositionX = value;
            OnSettingChanged(nameof(InitialPositionX));
        }
    }

    private double initialPositionY;
    public double InitialPositionY
    {
        get { return initialPositionY; }
        set
        {
            initialPositionY = value;
            OnSettingChanged(nameof(InitialPositionY));
        }
    }

    private double initialWindowHeight;
    public double InitialWindowHeight
    {
        get { return initialWindowHeight; }
        set
        {
            initialWindowHeight = value;
            OnSettingChanged(nameof(InitialWindowHeight));
        }
    }

    private double initialWindowWidth;
    public double InitialWindowWidth
    {
        get { return initialWindowWidth; }
        set
        {
            initialWindowWidth = value;
            OnSettingChanged(nameof(InitialWindowWidth));
        }
    }

    private string messageSaveDirectory;
    public string MessageSaveDirectory
    {
        get { return messageSaveDirectory; }
        set
        {
            messageSaveDirectory = value;
            OnSettingChanged(nameof(MessageSaveDirectory));
        }
    }

    private string themeMode;
    public string ThemeMode
    {
        get { return themeMode; }
        set
        {
            themeMode = value;
            OnSettingChanged(nameof(ThemeMode));
        }
    }

    private string theme;
    public string Theme
    {
        get { return theme; }
        set
        {
            theme = value;
            OnSettingChanged(nameof(Theme));
        }
    }

    private bool isMarkdownEnabled;

    public bool IsMarkdownEnabled
    {
        get { return isMarkdownEnabled; }
        set
        {
            isMarkdownEnabled = value;
            OnSettingChanged(nameof(IsMarkdownEnabled));
        }
    }

    private readonly string filePath;
    public Action<string> SettingChanged = delegate{ };

    public AppSettings(string filePath)
    {
        this.filePath = filePath;
    }

    private void OnSettingChanged(string name)
    {
        SettingChanged(name);
        var options = new JsonSerializerOptions
        {
            WriteIndented = true
        };
        string json = JsonSerializer.Serialize(this,options);
        File.WriteAllText(filePath, json);
    }
}
