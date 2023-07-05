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
            UpdateJsonConfig(nameof(ApiKeys));
        }
    }

    private bool startWithWindows;
    public bool StartWithWindows
    {
        get { return startWithWindows; }
        set
        {
            startWithWindows = value;
            UpdateJsonConfig(nameof(StartWithWindows));
        }
    }

    private double initialPositionX;
    public double InitialPositionX
    {
        get { return initialPositionX; }
        set
        {
            initialPositionX = value;
            UpdateJsonConfig(nameof(InitialPositionX));
        }
    }

    private double initialPositionY;
    public double InitialPositionY
    {
        get { return initialPositionY; }
        set
        {
            initialPositionY = value;
            UpdateJsonConfig(nameof(InitialPositionY));
        }
    }

    private double initialWindowHeight;
    public double InitialWindowHeight
    {
        get { return initialWindowHeight; }
        set
        {
            initialWindowHeight = value;
            UpdateJsonConfig(nameof(InitialWindowHeight));
        }
    }

    private double initialWindowWidth;
    public double InitialWindowWidth
    {
        get { return initialWindowWidth; }
        set
        {
            initialWindowWidth = value;
            UpdateJsonConfig(nameof(InitialWindowWidth));
        }
    }

    private string messageSaveDirectory;
    public string MessageSaveDirectory
    {
        get { return messageSaveDirectory; }
        set
        {
            messageSaveDirectory = value;
            UpdateJsonConfig(nameof(MessageSaveDirectory));
        }
    }

    private string themeMode;
    public string ThemeMode
    {
        get { return themeMode; }
        set
        {
            themeMode = value;
            UpdateJsonConfig(nameof(ThemeMode));
        }
    }

    private string theme;
    public string Theme
    {
        get { return theme; }
        set
        {
            theme = value;
            UpdateJsonConfig(nameof(Theme));
        }
    }

    private bool isMarkdownEnabled;

    public bool IsMarkdownEnabled
    {
        get { return isMarkdownEnabled; }
        set
        {
            isMarkdownEnabled = value;
            UpdateJsonConfig(nameof(IsMarkdownEnabled));
        }
    }

    private readonly string filePath;
    public Action<string> SettingChanged = delegate{ };

    public AppSettings(string filePath)
    {
        this.filePath = filePath;
    }

    private void UpdateJsonConfig(string name)
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
