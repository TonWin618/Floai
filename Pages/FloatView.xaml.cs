using Floai.Utils;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Floai.Pages
{
    public partial class FloatView : Window
    {
        static ChatView? chatView;
        public FloatView()
        {
            InitializeComponent();
            double window_x = AppConfiger.GetValue<double>("initialPositionX");
            double window_y = AppConfiger.GetValue<double>("initialPositionY");
            this.Left = window_x;
            this.Top = window_y;
        }

        private void SwitchToChatWindow()
        {
            if (chatView == null)
                chatView = new ChatView();

            double windowHeight = AppConfiger.GetValue<double>("initialWindowHeight");
            double windowWidth = AppConfiger.GetValue<double>("initialWindowWidth");

            chatView.Left = this.Left - windowWidth + this.Width;
            chatView.Top = this.Top - windowHeight + this.Height;
            chatView.Width = windowWidth;
            chatView.Height = windowHeight;

            this.Visibility = Visibility.Collapsed;
            chatView.Visibility = Visibility.Visible;
        }

        private void FloatingBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Indicate that the event has been handled
            e.Handled= true;

            Point beforeDragMovePosition = new Point(this.Left, this.Top);
            if (e.LeftButton== MouseButtonState.Pressed)
                this.DragMove();
            Point afterDragMovePosition = new Point(this.Left, this.Top);

            // If the window position remains the same, it means the user wants to perform a click operation
            if (beforeDragMovePosition == afterDragMovePosition)
            {
                SwitchToChatWindow();
            }
            else
            {
                AppConfiger.SetValue("initialPositionX", this.Left.ToString());
                AppConfiger.SetValue("initialPositionY", this.Top.ToString());
            }
        }

        private void FloatingBorder_MouseLeave(object sender, MouseEventArgs e)
        {
            var brush = FindResource("btn_main_bg_brush") as SolidColorBrush;
            if (brush != null)
                FloatingBorder.Background = brush;
        }

        private void FloatingBorder_MouseEnter(object sender, MouseEventArgs e)
        {
            var brush = FindResource("btn_main_hover_bg_brush") as SolidColorBrush;
            if (brush != null)
                FloatingBorder.Background = brush;
        }
    }
}
