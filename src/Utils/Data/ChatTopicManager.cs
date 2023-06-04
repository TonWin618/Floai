using Floai.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Floai.Utils.Data;

public class ChatTopicManager
{
    private readonly string directoryPath;
    public readonly string fileExtension = "txt";
    private readonly int topicNameLenthLimit = 80;
    private readonly string fileNameSeparator = "-";
    private readonly string dateFormatString = "yyyyMMddHHmmss";
    public ChatTopicManager(string directoryPath)
    {
        this.directoryPath = directoryPath;
    }

    public List<ChatTopic> GetChatTopics()
    {
        List<ChatTopic> chatTopics = new List<ChatTopic>();
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        string[] fileNameList = Directory.GetFiles(directoryPath, $"*.{fileExtension}").Select(Path.GetFileName).ToArray();

        foreach (var fileName in fileNameList)
        {
            chatTopics.Add(ParseFileName(fileName));
        }

        return chatTopics;
    }

    public ChatTopic CreateChatTopic(string firstMsg)
    {
        DateTime dateTime = DateTime.Now;
        string dateTimeString = dateTime.ToString(dateFormatString);

        StringBuilder topicNameStringBuilder = new StringBuilder(Math.Min(topicNameLenthLimit, firstMsg.Length));
        char[] invalidChars = Path.GetInvalidFileNameChars();
        for (int i = 0; i < Math.Min(topicNameLenthLimit, firstMsg.Length); i++)
        {
            if (!invalidChars.Contains(firstMsg[i]))
            {
                topicNameStringBuilder.Append(firstMsg[i]);
            }
        }
        string topicName = topicNameStringBuilder.ToString();
        string fileName = $"{dateTimeString}{fileNameSeparator}{topicName}.{fileExtension}";
        string filePath = Path.Combine(directoryPath, fileName);

        using (File.Create(filePath)) { }
        return new ChatTopic(dateTime, topicName, filePath);
    }

    public ChatTopic ParseFileName(string fileName)
    {

        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string filePath = Path.Combine(directoryPath, fileName);

        int dateTimeLenth = new DateTime().ToString(dateFormatString).Length;

        string formattedDateTime = fileNameWithoutExtension.Substring(0, dateTimeLenth);
        DateTime dateTime;
        if (!DateTime.TryParseExact(formattedDateTime, dateFormatString, null, System.Globalization.DateTimeStyles.None, out dateTime))
        {
            throw new ArgumentException("Invalid date time format.");
        }

        string topicName = fileNameWithoutExtension.Substring(dateTimeLenth + fileNameSeparator.Length);

        return new ChatTopic(dateTime, topicName, filePath);
    }
}
