using Floai.Utils.App;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace Floai.Pages
{
    public class WindowsTaskbarIcon
    {
        TaskbarIcon WindowsNotifyIcon { get; set; }
        private readonly WindowManager windowManager;
        public WindowsTaskbarIcon(WindowManager windowManager)
        {
            this.windowManager = windowManager;
        }

        public void Open()
        {
            if (WindowsNotifyIcon == null)
            {
                InitNotifyIcon();
            }
        }

        void InitNotifyIcon()
        {
            WindowsNotifyIcon = new TaskbarIcon
            {
                Icon = new Icon("./Floai.ico")
            };
            ContextMenu context = new();

            WindowsNotifyIcon.TrayLeftMouseDown += delegate (object sender, RoutedEventArgs e)
            {
                var chatView = windowManager.FindWindow<ChatView>();
                if (chatView != null)
                {
                    chatView.Visibility = Visibility.Visible;
                }
                else
                {
                    chatView.Show();
                }
            };

            MenuItem settings = new()
            {
                Header = "Settings",
                FontSize = 12
            };
            settings.Click += delegate (object sender, RoutedEventArgs e)
            {
                var settingsView = windowManager.FindWindow<SettingsView>();
                if (settingsView != null)
                {
                    settingsView.Activate();
                }
                else
                {
                    settingsView.Show();
                }
            };
            context.Items.Add(settings);

            MenuItem exit = new()
            {
                Header = "Exit",
                FontSize = 12
            };
            exit.Click += delegate (object sender, RoutedEventArgs e)
            {
                Environment.Exit(0);
            };
            context.Items.Add(exit);

            WindowsNotifyIcon.ContextMenu = context;
        }
    }
}