using Floai.Models;
using Floai.Utils.View;
using Floai.Utils.Model;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using Floai.ApiClients.abs;
using System.ComponentModel;

namespace Floai.Pages;

public partial class ChatView : Window, ISetWindowProperties
{
    public ChatViewModel viewModel;
    private readonly WindowManager windowManager;
    private readonly GeneralSettings appSettings;

    private bool autoScrollEnabled = true;
    private ScrollViewer? scrollViewer;
    private ChatBubbleSelector chatBubbleSelector;
    public ChatView(WindowManager windowManager, GeneralSettings appSettings, BaseApiClient apiClient)
    {
        this.windowManager = windowManager;
        this.appSettings = appSettings;
        InitializeComponent();

        chatBubbleSelector = new ChatBubbleSelector(appSettings.IsMarkdownEnabled);
        MessageList.ItemTemplateSelector = chatBubbleSelector;
        appSettings.PropertyChanged += OnSettingChanged;

        viewModel = new ChatViewModel(this.ScrollToBottom, appSettings, apiClient);
        this.DataContext = viewModel;
        
    }

    public void OnSettingChanged(object sender, PropertyChangedEventArgs e)
    {
        if(e.PropertyName == nameof(appSettings.IsMarkdownEnabled))
        {
            chatBubbleSelector.isMarkdownEnabled = appSettings.IsMarkdownEnabled;
        }
    }

    public void SetWindowProperties(WindowProperties properties)
    {
        (this.Width, this.Height) = viewModel.ReadWindowSize();
        this.Left = properties.Right - this.Width;
        this.Top = properties.Bottom - this.Height;
        this.Visibility = Visibility.Visible;
    }

    private void BtnDrag_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private async void BtnSend_Click(object sender, RoutedEventArgs e)
    {
        autoScrollEnabled = true;
        await viewModel.RequestAndReceiveResponse();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        this.Visibility = Visibility.Collapsed;
        windowManager.SetWindow<FloatView>(new WindowProperties(this));
    }

    private void BtnNewChat_Click(object sender, RoutedEventArgs e)
    {
        viewModel.BeforeCreateNewTopic();
    }

    private void TopicCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        autoScrollEnabled = true;
        viewModel.SwitchTopic();
        ScrollToBottom();
    }

    private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
    {

        if (e.Key == Key.Enter && Keyboard.Modifiers.HasFlag(ModifierKeys.Control))
        {
            e.Handled = true;
            FocusManager.SetFocusedElement(this, BtnSend);//Transfer focus to notify the variable bound to the input box to update its value.
            BtnSend?.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
        }
    }

    private void Window_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        viewModel.WriteWindowSize(this.Width, this.Height);
        MessageList.MaxHeight = this.Height - 67;//67 represents the height from the top of the input box to the bottom of the window.
    }

    private void ScrollToBottom()
    {
        if (!autoScrollEnabled) return;

        if (MessageList.Items.Count > 0)
        {
            var lastItem = MessageList.Items[^1];
            MessageList.ScrollIntoView(lastItem);
        }
    }

    private void MessageList_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
    {
        e.Handled = true;
        autoScrollEnabled = false;
        scrollViewer = FindVisualChild<ScrollViewer>(MessageList);
        scrollViewer.ScrollToVerticalOffset(scrollViewer.VerticalOffset - e.Delta);
    }

    private T? FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
    {
        for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
        {
            DependencyObject child = VisualTreeHelper.GetChild(obj, i);
            if (child != null && child is T t)
            {
                return t;
            }
            else
            {
                T grandChild = FindVisualChild<T>(child);
                if (grandChild != null)
                {
                    return grandChild;
                }
            }
        }
        return null;
    }
}
