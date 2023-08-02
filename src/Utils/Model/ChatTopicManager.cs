using Floai.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Floai.Utils.Model;

public class ChatTopicManager
{
    /// <summary>
    /// Manages chat topics by creating, parsing, and retrieving chat topic information based on file names.
    /// </summary>
    public string DirectoryPath
    {
        get;
        set;
    }
    public readonly string fileExtension = "json";
    private readonly int topicNameLenthLimit = 80;
    private readonly string fileNameSeparator = "-";
    private readonly string dateFormatString = "yyyyMMddHHmmss";

    /// <summary>
    /// Initializes a new instance of the ChatTopicManager class with the specified directory path.
    /// </summary>
    /// <param name="directoryPath">The path of the directory where chat topics will be stored.</param>
    public ChatTopicManager(string directoryPath)
    {
        this.DirectoryPath = directoryPath;
    }

    /// <summary>
    /// Retrieves a list of all chat topics stored in the specified directory.
    /// </summary>
    /// <returns>A list of ChatTopic objects representing the chat topics.</returns>
    public List<ChatTopic> GetChatTopics()
    {
        List<ChatTopic> chatTopics = new();
        if (!Directory.Exists(DirectoryPath))
        {
            Directory.CreateDirectory(DirectoryPath);
        }
        string[] fileNameList = Directory.GetFiles(DirectoryPath, $"*.{fileExtension}").Select(Path.GetFileName).ToArray();

        foreach (var fileName in fileNameList)
        {
            chatTopics.Add(ParseFileName(fileName));
        }

        return chatTopics;
    }

    /// <summary>
    /// Creates a new chat topic based on the provided initial message and saves it as a file in the specified directory.
    /// </summary>
    /// <param name="firstMsg">The initial message of the chat topic.</param>
    /// <returns>A new ChatTopic object representing the created chat topic.</returns>
    public ChatTopic CreateChatTopic(string firstMsg)
    {
        DateTime dateTime = DateTime.Now;
        string dateTimeString = dateTime.ToString(dateFormatString);

        StringBuilder topicNameStringBuilder = new(Math.Min(topicNameLenthLimit, firstMsg.Length));
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
        string filePath = Path.Combine(DirectoryPath, fileName);

        using (File.Create(filePath)) { }
        return new ChatTopic(dateTime, topicName, filePath);
    }

    /// <summary>
    /// Parses the provided file name and extracts the chat topic's date, name, and file path.
    /// </summary>
    /// <param name="fileName">The file name to parse.</param>
    /// <returns>A ChatTopic object representing the chat topic information.</returns>
    public ChatTopic ParseFileName(string fileName)
    {
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
        string filePath = Path.Combine(DirectoryPath, fileName);

        int dateTimeLenth = new DateTime().ToString(dateFormatString).Length;

        string formattedDateTime = fileNameWithoutExtension[..dateTimeLenth];
        if (!DateTime.TryParseExact(formattedDateTime, dateFormatString, null, System.Globalization.DateTimeStyles.None, out DateTime dateTime))
        {
            throw new ArgumentException("Invalid date time format.");
        }

        string topicName = fileNameWithoutExtension[(dateTimeLenth + fileNameSeparator.Length)..];

        return new ChatTopic(dateTime, topicName, filePath);
    }
}
