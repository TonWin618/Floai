using Floai.Models;
using Floai.Utils.View;
using Floai.Utils.Model;
using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace Floai.Pages
{
    public partial class FloatView : Window,ISetWindowProperties
    {
        readonly FloatViewModel viewModel;
        SolidColorBrush? borderDefaultBrush;
        SolidColorBrush? borderHoverbrush;
        private readonly WindowManager windowManager;
        public FloatView(WindowManager windowManager, FloatViewModel viewModel)
        {
            this.windowManager = windowManager;
            InitializeComponent();
            LoadResources();
            this.viewModel = viewModel;
            this.ShowInTaskbar = false;
        }

        public void SetWindowProperties(WindowProperties properties)
        {
            if(properties == null)
            {
                (this.Left, this.Top) = viewModel.ReadWindowPostion();
                return;
            }

            if ((SystemParameters.PrimaryScreenWidth < this.Left - this.Width) || (SystemParameters.PrimaryScreenHeight < this.Top - this.Height))
            {
                this.Left = SystemParameters.PrimaryScreenWidth / 2;
                this.Top = SystemParameters.PrimaryScreenHeight / 2;
            }
            this.Left = properties.Right - this.Width;
            this.Top = properties.Bottom - this.Height;
            this.Visibility = Visibility.Visible;
        }

        private void SwitchToChatWindow()
        {
            this.Visibility = Visibility.Collapsed;
            windowManager.SetWindow<ChatView>(new WindowProperties(this));
        }

        private void FloatingBorder_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Indicate that the event has been handled
            e.Handled = true;

            Point beforeDragMovePosition = new(this.Left, this.Top);
            if (e.LeftButton == MouseButtonState.Pressed)
                this.DragMove();
            Point afterDragMovePosition = new(this.Left, this.Top);

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
