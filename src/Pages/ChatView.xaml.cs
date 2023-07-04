using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Floai.Pages;

public partial class ChatView : Window
{
    public ChatViewModel viewModel;
    private static FloatView? floatView;
    private bool autoScrollEnabled = true;
    ScrollViewer? scrollViewer;
    public ChatView()
    {
        InitializeComponent();
        viewModel = new ChatViewModel(this.ScrollToBottom);
        this.DataContext = viewModel;
        (this.Width, this.Height) = viewModel.ReadWindowSize();
        
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
        floatView ??= new FloatView();
        floatView.Left = this.Left + this.Width - floatView.Width;
        floatView.Top = this.Top + this.Height - floatView.Height;
        floatView.Closed += (s, evenArgs) => floatView = null;
        this.Visibility = Visibility.Collapsed;
        floatView.Visibility = Visibility.Visible;
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

    private T FindVisualChild<T>(DependencyObject obj) where T : DependencyObject
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
