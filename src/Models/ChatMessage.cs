using System;
using System.ComponentModel;

namespace Floai.Model;
public enum Sender
{
    AI,
    User
}
public class ChatMessage : INotifyPropertyChanged
{
    public DateTime DateTime { get; set; }
    public Sender Sender { get; set; }
    public string Content { get; set; }

    public ChatMessage(DateTime dateTime, Sender sender, string content)
    {
        this.DateTime = dateTime;
        this.Sender = Sender.AI;
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