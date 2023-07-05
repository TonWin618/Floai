using Floai.Model;
using Floai.Models;
using System.Windows;
using System.Windows.Controls;

namespace Floai.Utils.Data;

class ChatBubbleSelector : DataTemplateSelector
{
    public bool isMarkdownEnabled = true;
    //When declaring an instance of DataTemplateSelector in XAML and assigning it to the ItemTemplateSelector property, the constructor will not be automatically invoked.
    public ChatBubbleSelector(bool isMarkdownEnabled)
    {
        this.isMarkdownEnabled = isMarkdownEnabled;
    }
    public override DataTemplate SelectTemplate(object item, DependencyObject container)
    {
        var u = container as FrameworkElement;

        ChatMessage message = item as ChatMessage;
        if (message.Sender == Sender.User)
            return u.FindResource("user") as DataTemplate;
        else if (message.Sender == Sender.AI && isMarkdownEnabled)
            return u.FindResource("ai-markdown") as DataTemplate;
        else
            return u.FindResource("ai-text") as DataTemplate;
    }
}
