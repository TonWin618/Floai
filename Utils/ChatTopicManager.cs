using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Floai.Utils;

public class ChatTopicManager
{
    private readonly string directoryPath;

    public ChatTopicManager(string directoryPath)
    {
        this.directoryPath = directoryPath;
    }

    public List<string> GetMessageLogFileNames()
    {
        return Directory.GetFiles(directoryPath, "*.txt").Select(Path.GetFileName).ToList();
    }
}
