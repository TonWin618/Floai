using Floai.Pages;
using Floai.Utils.App;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;
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
        WindowsNotifyIcon = new TaskbarIcon();
        WindowsNotifyIcon.Icon = new Icon("./Floai.ico");
        ContextMenu context = new ContextMenu();

        MenuItem settings = new MenuItem();
        settings.Header = "Settings";
        settings.FontSize = 14;
        settings.Click += delegate (object sender, RoutedEventArgs e)
        {
            if (WindowHelper.IsWindowOpen<SettingsView>())
                return;
            var settingsView = new SettingsView();
            settingsView.Show();
        };
        context.Items.Add(settings);

        MenuItem exit = new MenuItem();
        exit.Header = "Exit";
        exit.FontSize = 14;
        exit.Click += delegate (object sender, RoutedEventArgs e)
        {
            Environment.Exit(0);
        };
        context.Items.Add(exit);

        WindowsNotifyIcon.ContextMenu = context;
    }
}