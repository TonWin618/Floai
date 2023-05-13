﻿using System.Windows;

namespace Floai
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            WindowsTaskbarIcon.Open();
        }
    }
}
