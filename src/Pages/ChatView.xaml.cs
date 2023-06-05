using Floai.Utils.App;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Floai.Pages;

public partial class ChatView : Window
{
    public ChatViewModel viewModel;
    private static FloatView? floatView;
    public ChatView()
    {
        InitializeComponent();
        viewModel = new ChatViewModel(this.ScrollToBottom);
        this.DataContext = viewModel;
        (this.Width, this.Height) = viewModel.ReadWindowSize();
        TransparentClick.Enable(this);
    }

    private void BtnDrag_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        DragMove();
    }

    private async void BtnSend_Click(object sender, RoutedEventArgs e)
    {
        await viewModel.RequestAndReceiveResponse();
    }

    private void BtnSetting_Click(object sender, RoutedEventArgs e)
    {
        ShowSettingsView();
    }

    private void BtnClose_Click(object sender, RoutedEventArgs e)
    {
        if (floatView == null)
        {
            floatView = new FloatView();
        }
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
        viewModel.LoadMessages();
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
    }

    private void ShowSettingsView()
    {
        if (WindowHelper.IsWindowOpen<SettingsView>())
            return;
        var settingsView = new SettingsView();
        settingsView.Show();
    }

    private void ScrollToBottom()
    {
        if (MessageList.Items.Count > 0)
        {
            var lastItem = MessageList.Items[MessageList.Items.Count - 1];
            MessageList.ScrollIntoView(lastItem);
        }
    }
}
