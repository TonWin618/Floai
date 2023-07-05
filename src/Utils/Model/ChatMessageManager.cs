using Floai.Model;
using System.Text.Json;
using System.Collections.Generic;
using System.IO;

namespace Floai.Utils.Model;

public class ChatMessageManager
{
    private string filePath;

    // Constructor for MessageManager class. It takes file path as input parameter.
    public ChatMessageManager(string filePath)
    {
        this.filePath = filePath;
    }

    // Save a single chat message to the file
    public void SaveMessage(ChatMessage message)
    {
        EnsureFileExists();

        using (StreamWriter writer = new StreamWriter(filePath, true))
        {
            string line = JsonSerializer.Serialize(message);
            writer.WriteLine(line);
        }
    }

    // Save list of chat messages to the file
    public void SaveMessages(List<ChatMessage> messages)
    {
        EnsureFileExists();

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            foreach (ChatMessage message in messages)
            {
                string line = JsonSerializer.Serialize(message);
                writer.WriteLine(line);
            }
        }
    }

    // Load list of chat messages from the file
    public List<ChatMessage> LoadMessages()
    {
        EnsureFileExists();

        List<ChatMessage> messages = new List<ChatMessage>();

        using (StreamReader reader = new StreamReader(filePath))
        {
            while (!reader.EndOfStream)
            {
                string line = reader.ReadLine();

                ChatMessage message = JsonSerializer.Deserialize<ChatMessage>(line);

                messages.Add(message);
            }
        }

        return messages;
    }

    // Ensure that file and its parent directory exist, create them if they do not exist
    private void EnsureFileExists()
    {
        if (!File.Exists(filePath))
        {
            using (File.Create(filePath)) { };
        }
    }
}

