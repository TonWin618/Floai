using System;
using System.IO;

namespace Floai.Utils.Model;

public class FileWatcher
{
    private readonly FileSystemWatcher watcher;
    /// <summary>
    /// Initializes a new instance of the FileWatcher class with the specified file path and action to perform on file changes.
    /// </summary>
    /// <param name="filePath">The path of the directory to be monitored.</param>
    /// <param name="action">The action to be executed when a file is created or deleted.</param>
    public FileWatcher(string filePath, Action<object, FileSystemEventArgs> action)
    {
        // Create a new instance of FileSystemWatcher and set its properties.
        watcher = new FileSystemWatcher
        {
            Path = filePath,
            Filter = "*.*", // Monitor all file types in the specified directory.
            NotifyFilter = NotifyFilters.FileName // Only notify when file names are changed (created or deleted).
        };

        // Attach the provided action to both the Created and Deleted events.
        watcher.Created += new FileSystemEventHandler(action);
        watcher.Deleted += new FileSystemEventHandler(action);

        // Start raising events for file changes.
        watcher.EnableRaisingEvents = true;
    }
}
