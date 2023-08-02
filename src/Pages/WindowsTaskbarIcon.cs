using Floai.Utils.View;
using Hardcodet.Wpf.TaskbarNotification;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Controls;

namespace Floai.Pages;

/// <summary>
/// Represents a Windows taskbar icon with associated functionality and context menu.
/// </summary>
public class WindowsTaskbarIcon
{
    TaskbarIcon WindowsNotifyIcon { get; set; }
    private readonly WindowManager windowManager;

    /// <summary>
    /// Initializes a new instance of the WindowsTaskbarIcon class with the specified WindowManager.
    /// </summary>
    /// <param name="windowManager">The WindowManager instance used for managing application windows.</param>
    public WindowsTaskbarIcon(WindowManager windowManager)
    {
        this.windowManager = windowManager;
    }

    /// <summary>
    /// Opens the Windows taskbar icon.
    /// </summary>
    public void Open()
    {
        if (WindowsNotifyIcon == null)
        {
            InitNotifyIcon();
        }
    }

    /// <summary>
    /// Initializes the TaskbarIcon and sets its properties and context menu items.
    /// </summary>
    void InitNotifyIcon()
    {
        // Create a new TaskbarIcon instance.
        WindowsNotifyIcon = new TaskbarIcon
        {
            Icon = new Icon("./Floai.ico")
        };


        // Create a new context menu for the taskbar icon.
        ContextMenu context = new();

        // Event handler for left mouse button click on the taskbar icon.
        WindowsNotifyIcon.TrayLeftMouseDown += delegate (object sender, RoutedEventArgs e)
        {
            // Find existing windows instances of ChatView and FloatView.
            var chatView = windowManager.FindWindow<ChatView>();
            var floatView = windowManager.FindWindow<FloatView>();

            // If FloatView exists, collapse it; otherwise, return.
            if (floatView != null)
                floatView.Visibility = Visibility.Collapsed;
            else
                return;

            // If ChatView exists, make it visible; otherwise, create a new instance of ChatView.
            if (chatView != null)
                chatView.Visibility = Visibility.Visible;
            else
                windowManager.SetWindow<ChatView>(new WindowProperties(floatView));
        };


        // Create a "Settings" menu item and set its click event handler.
        MenuItem settings = new()
        {
            Header = "Settings",
            FontSize = 12
        };
        settings.Click += delegate (object sender, RoutedEventArgs e)
        {
            // Find existing instance of SettingsView, activate it if found, otherwise create a new instance.
            var settingsView = windowManager.FindWindow<SettingsView>();
            if (settingsView != null)
            {
                settingsView.Activate();
            }
            else
            {
                windowManager.SetWindow<SettingsView>(null);
            }
        };
        context.Items.Add(settings);


        // Create an "Exit" menu item and set its click event handler.
        MenuItem exit = new()
        {
            Header = "Exit",
            FontSize = 12
        };
        exit.Click += delegate (object sender, RoutedEventArgs e)
        {
            // Exit the application.
            Environment.Exit(0);
        };
        context.Items.Add(exit);

        // Set the context menu for the TaskbarIcon.
        WindowsNotifyIcon.ContextMenu = context;
    }
}