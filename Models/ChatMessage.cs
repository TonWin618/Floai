using System;
using System.ComponentModel;

namespace Floai.Model;

public class ChatMessage: INotifyPropertyChanged
{
    public DateTime DateTime { get; set; }
    public string Sender { get; set; }
    public string Content{get; set; }

    public ChatMessage(DateTime dateTime, string sender, string content)
    {
        this.DateTime = dateTime;
        this.Sender = sender;
        this.Content = content;
    }

    public void AppendContent(string delta)
    {
        this.Content += delta;
        //Make sure the view can be refreshed
        PropertyChanged(this, new PropertyChangedEventArgs("Content"));
    }

    public event PropertyChangedEventHandler? PropertyChanged = delegate { };
}