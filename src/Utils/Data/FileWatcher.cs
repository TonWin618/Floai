using System;
using System.IO;

namespace Floai.Utils.Data;

public class FileWatcher
{
    private readonly FileSystemWatcher watcher;
    public FileWatcher(string filePath, Action<object, FileSystemEventArgs> action)
    {
        watcher = new FileSystemWatcher
        {
            Path = filePath,
            Filter = "*.*",
            NotifyFilter = NotifyFilters.FileName
        };
        watcher.Created += new FileSystemEventHandler(action);
        watcher.Deleted += new FileSystemEventHandler(action);
        watcher.EnableRaisingEvents = true;
    }
}
