using Floai.Pages;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace Floai.Utils.App
{
    public static class WindowsTaskbarIcon
    {
        static TaskbarIcon WindowsNotifyIcon { get; set; }

        public static void Open()
        {
            if (WindowsNotifyIcon == null)
            {
                InitNotifyIcon();
            }
        }

        static void InitNotifyIcon()
        {
            WindowsNotifyIcon = new TaskbarIcon
            {
                Icon = new Icon("./Floai.ico")
            };
            ContextMenu context = new();

            WindowsNotifyIcon.TrayLeftMouseDown += delegate (object sender, RoutedEventArgs e)
            {
                var chatView = WindowHelper.FindiWindow<ChatView>();
                if (chatView != null)
                {
                    chatView.Visibility = Visibility.Visible;
                }
                else
                {
                    chatView = new ChatView();
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
                var settingsView = WindowHelper.FindiWindow<SettingsView>();
                if (settingsView != null)
                {
                    settingsView.Activate();
                }
                else
                {
                    settingsView = new SettingsView();
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