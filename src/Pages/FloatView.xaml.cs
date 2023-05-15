using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Floai.Pages
{
    public partial class FloatView : Window
    {
        FloatViewModel viewModel;
        static ChatView? chatView;
        SolidColorBrush? borderDefaultBrush;
        SolidColorBrush? borderHoverbrush;
        public FloatView()
        {
            InitializeComponent();
            this.ShowInTaskbar = false;
            viewModel = new FloatViewModel();
            (this.Left, this.Top) = viewModel.ReadWindowPostion();
            LoadResources();
        }

        private void SwitchToChatWindow()
        {
            if (chatView == null)
                chatView = new ChatView();

            chatView.Left = this.Left - chatView.Width + this.Width;
            chatView.Top = this.Top - chatView.Height + this.Height;

            this.Visibility = Visibility.Collapsed;
            chatView.Visibility = Visibility.Visible;
        }

        private void FloatingBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Indicate that the event has been handled
            e.Handled = true;

            Point beforeDragMovePosition = new Point(this.Left, this.Top);
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
            Point afterDragMovePosition = new Point(this.Left, this.Top);

            // If the window position remains the same, it means the user wants to perform a click operation
            if (beforeDragMovePosition == afterDragMovePosition)
            {
                SwitchToChatWindow();
            }
            else
            {
                viewModel.WriteWindowPostion(this.Left, this.Top);
            }
        }

        private void FloatingBorder_MouseLeave(object sender, MouseEventArgs e)
        {

            if (borderDefaultBrush != null)
                FloatingBorder.Background = borderDefaultBrush;
        }

        private void FloatingBorder_MouseEnter(object sender, MouseEventArgs e)
        {

            if (borderHoverbrush != null)
                FloatingBorder.Background = borderHoverbrush;
        }

        private void LoadResources()
        {
            borderDefaultBrush = FindResource("btn_main_bg_brush") as SolidColorBrush;
            borderHoverbrush = FindResource("btn_main_hover_bg_brush") as SolidColorBrush;
        }
    }
}
