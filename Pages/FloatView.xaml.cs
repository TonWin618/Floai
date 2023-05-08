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
        public FloatView()
        {
            InitializeComponent();
            double window_x = AppConfiger.GetValue<double>("initialPositionX");
            double window_y = AppConfiger.GetValue<double>("initialPositionY");
            this.Left = window_x;
            this.Top = window_y;
        }

        private void ShowWindow()
        {
            if (chatView == null)
                chatView = new ChatView();
            double window_height = AppConfiger.GetValue<double>("initialWindowHeight");
            double window_width = AppConfiger.GetValue<double>("initialWindowWidth");

            chatView.Left = this.Left - window_width + 80;
            chatView.Top = this.Top - window_height + 30;
            chatView.Width = window_width;
            chatView.Height = window_height;
            chatView.Closed += (s, evenArgs) => chatView = null;

            AppConfiger.SetValue("initialPositionX", this.Left.ToString());
            AppConfiger.SetValue("initialPositionY", this.Top.ToString());

            this.Visibility = Visibility.Collapsed;
            chatView.Visibility = Visibility.Visible;
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled= true;
            if(e.LeftButton== MouseButtonState.Pressed) 
                this.DragMove();
            if(e.LeftButton== MouseButtonState.Released)
                ShowWindow();
        }

        private void border_MouseLeave(object sender, MouseEventArgs e)
        {
            var brush = FindResource("btn_main_bg_brush") as SolidColorBrush;
            if (brush != null)
                border.Background = brush;
        }

        private void border_MouseEnter(object sender, MouseEventArgs e)
        {
            var brush = FindResource("btn_main_hover_bg_brush") as SolidColorBrush;
            if (brush != null)
                border.Background = brush;
        }
    }
}
