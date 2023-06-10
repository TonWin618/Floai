using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;

namespace Floai.Utils.Data;
public static class AppConfiger
{
    private static readonly string configFilePath;
    private static readonly XmlDocument xmlDoc;
    private static readonly string rootNodeName = "configuration";
    private static readonly char nodeSeparator = '/';
    public delegate void SettingChangedEventHandler(string key, string value);
    public static event SettingChangedEventHandler SettingChanged = delegate{ };

    static AppConfiger()
    {
        configFilePath = "App.config";
        xmlDoc = new XmlDocument();
        XmlReaderSettings settings= new XmlReaderSettings();
        XmlReader reader = XmlReader.Create(configFilePath, settings);
        xmlDoc.Load(reader);
        reader.Close();
    }

    /// <summary>
    /// Getting the value of a configuration item.
    /// </summary>
    /// <param name="key">Divide by "/" (eg. /apiKeys/apiKey)</param>
    /// <param name="defaultValue"></param>
    /// <returns></returns>
    public static string GetValue(string key, string defaultValue = null)
    {
        string fullKey = GetFullKey(key);
        return xmlDoc.SelectSingleNode(fullKey)?.InnerText ?? defaultValue;
    }
    public static T GetValue<T>(string key, T defaultValue = default)
    {
        var strValue = GetValue(key);
        if (string.IsNullOrEmpty(strValue))
        {
            return defaultValue;
        }
        return (T)Convert.ChangeType(strValue, typeof(T));
    }

    /// <summary>
    /// Setting the value of an existing configuration item.
    /// </summary>
    /// <param name="key">Divide by "/" (eg. /apiKeys/apiKey)</param>
    /// <param name="value"></param>
    public static void SetValue(string key, string value)
    {
        string fullKey = GetFullKey(key);
        XmlNode? node = xmlDoc.SelectSingleNode(fullKey);
        if (node == null)
        {
            throw new NullReferenceException("Cannot find the specified node.");
        }
        node.InnerText = value;
        SettingChanged(key, value);
        xmlDoc.Save(configFilePath);
    }

    /// <summary>
    /// Getting values of multiple configuration items with the same name.
    /// </summary>
    /// <param name="key">Divide by "/" (eg. /apiKeys/apiKey)</param>
    /// <returns></returns>
    public static IEnumerable<string> GetValues(string key)
    {
        string fullKey = GetFullKey(key);
        XmlNodeList? nodes = xmlDoc.SelectNodes(fullKey);
        if(nodes == null)
        {
            throw new NullReferenceException("Cannot find the specified node.");
        }
        foreach(XmlNode node in nodes)
        {
            yield return node.InnerText;
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="key">Divide by "/" (eg. /apiKeys/apiKey)</param>
    /// <returns></returns>
    public static IEnumerable<T> GetValues<T>(string key)
    {
        foreach(var value in GetValues(key))
        {
            yield return (T)Convert.ChangeType(value, typeof(T));
        }
    }

    /// <summary>
    /// Adding a new configuration item.
    /// </summary>
    /// <param name="key">Divide by "/" (eg. /apiKeys/apiKey)</param>
    /// <param name="value"></param>
    public static void AddValue(string key, string value)
    {
        string fullKey = GetFullKey(key);
        IEnumerable<string> nodeNames = fullKey.Split(nodeSeparator).ToList();
        string childName = nodeNames.Last();

        string parentKey = string.Join(nodeSeparator, nodeNames.SkipLast(1));
        XmlNode? parentNode = xmlDoc.SelectSingleNode(parentKey);
        if (parentNode == null)
        {
            throw new NullReferenceException("Cannot find the specified node.");
        }

        XmlNode childNode = xmlDoc.CreateElement(childName);
        childNode.InnerText = value;
        parentNode.AppendChild(childNode);
        SettingChanged(key, value);
        xmlDoc.Save(configFilePath);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="key">Divide by "/" (eg. /apiKeys/apiKey)</param>
    /// <param name="value"></param>
    /// <exception cref="NullReferenceException"></exception>
    public static void RemoveValue(string key, string value)
    {
        string fullKey = GetFullKey(key);
        XmlNodeList? nodes = xmlDoc.SelectNodes(fullKey);
        if (nodes == null)
        {
            throw new NullReferenceException("Cannot find the specified node.");
        }

        if(nodes.Count == 0)
        {
            return;
        }

        XmlNode parentNode = nodes[0]!.ParentNode!;

        foreach (XmlNode node in nodes)
        {
            if(node.InnerText == value)
            {
                parentNode.RemoveChild(node);
            }
        }
        SettingChanged(key, value);
        xmlDoc.Save(configFilePath);
    }

    private static string GetFullKey(string key)
    {
        return rootNodeName + nodeSeparator + key;
    }
}