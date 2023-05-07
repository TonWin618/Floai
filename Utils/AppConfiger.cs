using System;
using System.Collections.Generic;
using System.Xml;

namespace Floai.Utils;
public class AppConfiger
{
    private readonly Dictionary<string, string> _configValues;
    private readonly string _configFilePath;
    public AppConfiger(string configFilePath)
    {
        _configFilePath = configFilePath;
        _configValues = new Dictionary<string, string>();
        // Load the XML file
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(configFilePath);

        // Get all the add elements and store their key/value pairs in the dictionary
        XmlNodeList addNodes = xmlDoc.SelectNodes("/configuration/add");
        foreach (XmlNode node in addNodes)
        {
            string key = node.Attributes["key"].Value;
            string value = node.Attributes["value"].Value;
            _configValues.Add(key, value);
        }
    }

    // Retrieve a configuration value by its key
    public string GetValue(string key)
    {
        if (_configValues.ContainsKey(key))
        {
            return _configValues[key];
        }
        else
        {
            throw new KeyNotFoundException($"The key '{key}' was not found in the configuration file.");
        }
    }

    // Retrieve a configuration value by its key, with a default value if the key is not found
    public string GetValue(string key, string defaultValue)
    {
        if (_configValues.ContainsKey(key))
        {
            return _configValues[key];
        }
        else
        {
            return defaultValue;
        }
    }

    // Retrieve a configuration value by its key, parsed to the specified type T
    public T GetValue<T>(string key)
    {
        string valueString = GetValue(key);
        Type type = typeof(T);

        try
        {
            // Convert the string value to the desired type
            if (type == typeof(int))
            {
                return (T)(object)int.Parse(valueString);
            }
            else if (type == typeof(bool))
            {
                return (T)(object)bool.Parse(valueString);
            }
            else if (type == typeof(double))
            {
                return (T)(object)double.Parse(valueString);
            }
            else if (type == typeof(float))
            {
                return (T)(object)float.Parse(valueString);
            }
            else if (type == typeof(string))
            {
                return (T)(object)valueString;
            }
            else
            {
                throw new NotSupportedException($"The type '{type.Name}' is not supported.");
            }
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"Error parsing value for key '{key}': {ex.Message}", ex);
        }
    }

    // Retrieve a configuration value by its key, parsed to the specified type T, with a default value if the key is not found
    public T GetValue<T>(string key, T defaultValue)
    {
        if (_configValues.ContainsKey(key))
        {
            return GetValue<T>(key);
        }
        else
        {
            return defaultValue;
        }
    }

    public void SetValue(string key, string value)
    {
        if (_configValues.ContainsKey(key))
        {
            _configValues[key] = value;
        }
        else
        {
            _configValues.Add(key, value);
        }

        // Save the updated config back to the original XML file
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(_configFilePath);
        XmlNodeList addNodes = xmlDoc.SelectNodes("/configuration/add[@key='" + key + "']");
        foreach (XmlNode addNode in addNodes)
        {
            addNode.Attributes["value"].Value = value;
        }
        xmlDoc.Save(_configFilePath);
    }

    // Save the current configuration back to the original XML file
    public void Save()
    {
        XmlDocument xmlDoc = new XmlDocument();
        xmlDoc.Load(_configFilePath);
        XmlNodeList addNodes = xmlDoc.SelectNodes("/configuration/add");
        foreach (XmlNode addNode in addNodes)
        {
            string key = addNode.Attributes["key"].Value;
            if (_configValues.ContainsKey(key))
            {
                addNode.Attributes["value"].Value = _configValues[key];
            }
        }
        xmlDoc.Save(_configFilePath);
    }
}
