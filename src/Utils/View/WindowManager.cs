using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Floai.Utils.View
{
    public class WindowManager
    {
        private readonly IServiceProvider serviceProvider;

        public WindowManager(IServiceProvider serviceProvider) 
        {
            this.serviceProvider = serviceProvider;
        }

        public Window? FindWindow<T>() where T : Window
        {
            return Application.Current.Windows.OfType<T>().FirstOrDefault();
        }

        public void SetWindow<T>(WindowProperties properties) where T : Window, ISetWindowProperties
        {
            var window = FindWindow<T>();
            window ??= this.serviceProvider.GetRequiredService<T>();
            ((ISetWindowProperties)window).SetWindowProperties(properties);
            window.Show();
        }
    }
}
