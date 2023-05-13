using System;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Floai.Utils;
public static class AppConfiger
{
    private static readonly string _configFilePath;
    private static readonly XDocument _configFile;

    static AppConfiger()
    {
        _configFilePath = "App.config";
        _configFile = LoadConfigFile(_configFilePath);
    }

    public static string GetValue(string key, string defaultValue = null)
    {
        var element = GetConfigElement(key);
        return element?.Attribute("value")?.Value ?? defaultValue;
    }

    public static T GetValue<T>(string key, T defaultValue = default)
    {
        var strValue = GetValue(key);
        if (string.IsNullOrEmpty(strValue))
        {
            return defaultValue;
        }
        else
        {
            return (T)Convert.ChangeType(strValue, typeof(T));
        }
    }

    public static void SetValue(string key, string value)
    {
        var element = GetConfigElement(key);
        if (element != null)
        {
            element.Attribute("value")?.SetValue(value);
        }
        else
        {
            var newElement = new XElement("add", new XAttribute("key", key), new XAttribute("value", value));
            _configFile.Root.Add(newElement);
        }
        SaveConfigFile();
    }

    private static XElement GetConfigElement(string key)
    {
        var elements = _configFile.Descendants("add");
        return elements.FirstOrDefault(e => e.Attribute("key")?.Value == key);
    }

    private static void SaveConfigFile()
    {
        _configFile.Save(_configFilePath);
    }

    private static XDocument LoadConfigFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Configuration file '{filePath}' does not exist.");
        }

        try
        {
            using var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            return XDocument.Load(fileStream, LoadOptions.PreserveWhitespace);
        }
        catch (Exception ex)
        {
            throw new Exception($"Failed to load configuration file '{filePath}', error message: {ex.Message}", ex);
        }
    }
}