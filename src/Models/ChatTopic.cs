using System;
using System.ComponentModel;

namespace Floai.Model;

public class ChatTopic : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged = delegate { };
    public DateTime DateTime { get; set; }

    private string name;
    public string Name
    {
        get
        {
            return this.name;
        }
        set
        {
            this.name = value;
            //Make sure the view can be refreshed
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(Name)));
        }
    }

    private string filePath;
    public string FilePath
    {
        get
        {
            return this.filePath;
        }
        set
        {
            this.filePath = value;
            //Make sure the view can be refreshed
            PropertyChanged(this, new PropertyChangedEventArgs(nameof(FilePath)));
        }
    }

    public ChatTopic(DateTime dateTime, string name, string filePath)
    {
        this.DateTime = dateTime;
        this.Name = name;
        this.FilePath = filePath;
    }
}