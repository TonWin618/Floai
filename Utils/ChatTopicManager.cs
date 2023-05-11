using Floai.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Floai.Utils;

public class ChatTopicManager
{
    private readonly string directoryPath;
    public readonly string fileExtension = "txt";

    public ChatTopicManager(string directoryPath)
    {
        this.directoryPath = directoryPath;
    }

    public List<ChatTopic> GetChatTopics()
    {
        List<ChatTopic> chatTopics = new List<ChatTopic>();
        string[] fileNameList = Directory.GetFiles(directoryPath, $"*.{fileExtension}").Select(Path.GetFileName).ToArray();

        foreach(var fileName in fileNameList)
        {
            chatTopics.Add(ParseFileName(fileName));
        }

        return chatTopics;
    }

    public ChatTopic CreateChatTopic(string topicName)
    {
        DateTime dateTime = DateTime.Now;
        string fileName = $"{dateTime:yyyyMMddHHmmss}-{topicName}.{fileExtension}";
        string filePath = Path.Combine(directoryPath, fileName);
        using(File.Create(filePath))
        {

        }
        return new ChatTopic(dateTime, fileName, filePath);
    }

    public ChatTopic ParseFileName(string fileName)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string[] parts = fileNameWithoutExtension.Split('-');
        if (parts.Length != 2)
        {
            throw new ArgumentException("Invalid file name format.");
        }

        DateTime dateTime;
        if (!DateTime.TryParseExact(parts[0], "yyyyMMddHHmmss", null, System.Globalization.DateTimeStyles.None, out dateTime))
        {
            throw new ArgumentException("Invalid date time format.");
        }

        string name = parts[1];

        string filePath = Path.Combine(directoryPath, fileName);

        return new ChatTopic(dateTime, name, filePath);
    }
}
