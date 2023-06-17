using Floai.Model;
using System.Windows;
using System.Windows.Controls;

namespace Floai.Utils.Data
{
    class ChatBubbleSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var u = container as FrameworkElement;

            ChatMessage message = item as ChatMessage;
            AppConfiger.GetValue("isMarkdownEnabled");
            if (message.Sender == "user")
                return u.FindResource("user") as DataTemplate;
            else if (message.Sender == "ai" && AppConfiger.GetValue<bool>("isMarkdownEnabled"))
                return u.FindResource("ai-markdown") as DataTemplate;
            else
                return u.FindResource("ai-text") as DataTemplate;
        }
    }
}
