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

    public List<string> GetLogNameWithoutExtension()
    {
        return Directory.GetFiles(directoryPath, $"*.{fileExtension}").Select(Path.GetFileNameWithoutExtension).ToList();
    }
}
