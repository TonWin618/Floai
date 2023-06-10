using System;
using System.IO;

namespace Floai.Utils.Data;

public class FileWatcher
{
    private FileSystemWatcher watcher;
    public FileWatcher(string filePath, Action<object, FileSystemEventArgs> action)
    {
        watcher = new FileSystemWatcher();
        watcher.Path = filePath;
        watcher.Filter = "*.*";
        watcher.NotifyFilter = NotifyFilters.FileName;
        watcher.Created += new FileSystemEventHandler(action);
        watcher.Deleted += new FileSystemEventHandler(action);
        watcher.EnableRaisingEvents = true;
    }
}
