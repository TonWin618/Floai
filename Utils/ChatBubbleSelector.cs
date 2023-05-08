using Floai.Model;
using System.Windows;
using System.Windows.Controls;

namespace Floai.Utils
{
    class ChatBubbleSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var u = container as FrameworkElement;

            ChatMessage message = item as ChatMessage;

            if (message.Sender == "user")
                return u.FindResource("user") as DataTemplate;
            else if (message.Sender == "ai")
                return u.FindResource("ai") as DataTemplate;
            else
                return u.FindResource("ai") as DataTemplate;
        }
    }
}
