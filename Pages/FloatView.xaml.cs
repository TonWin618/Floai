using Floai.Utils;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Floai.Pages
{
    public partial class FloatView : Window
    {
        static ChatView? chatView;
        AppConfiger configer = new("config.xml");
        public FloatView()
        {
            InitializeComponent();
            double window_x = configer.GetValue<double>("initialPositionX");
            double window_y = configer.GetValue<double>("initialPositionY");
            //this.Left = SystemParameters.PrimaryScreenWidth - this.Width - 100;
            //this.Top = SystemParameters.PrimaryScreenHeight - this.Height - 100;
            this.Left = window_x;
            this.Top = window_y;
        }

        private void ShowWindow()
        {
            if (chatView == null)
            {
                chatView = new ChatView();
            }
            double window_height = configer.GetValue<double>("initialWindowHeight");
            double window_width = configer.GetValue<double>("initialWindowWidth");
            chatView.Left = this.Left - window_width + 80;
            chatView.Top = this.Top - window_height + 30;
            chatView.Width = window_width;
            chatView.Height = window_height;
            chatView.Closed += (s, evenArgs) => chatView = null;
            configer.SetValue("initialPositionX", this.Left.ToString());
            configer.SetValue("initialPositionY", this.Top.ToString());

            this.Visibility = Visibility.Collapsed;
            chatView.Visibility = Visibility.Visible;
        }
        private Point _previousWindowPosition;

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            _previousWindowPosition = new Point(Left, Top);
            DragMove();
        }


        private void Border_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (_previousWindowPosition == new Point(Left, Top))
            {
                ShowWindow();
            }
        }

        private void border_MouseLeave(object sender, MouseEventArgs e)
        {
            var brush = FindResource("btn_main_bg_brush") as SolidColorBrush;
            if (brush != null)
            {
                border.Background = brush;
            }
        }

        private void border_MouseEnter(object sender, MouseEventArgs e)
        {
            var brush = FindResource("btn_main_hover_bg_brush") as SolidColorBrush;
            if (brush != null)
            {
                border.Background = brush;
            }
        }
    }
}
