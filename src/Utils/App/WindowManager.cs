using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Windows;

namespace Floai.Utils.App
{
    public class WindowManager
    {
        private readonly IServiceProvider serviceProvider;

        public WindowManager(IServiceProvider serviceProvider) 
        {
            this.serviceProvider = serviceProvider;
            Type[] types = Assembly.GetExecutingAssembly().GetTypes();
            var matchingTypes = types.Where(t =>
                t.Namespace == "Floai.Pages" &&
                t.IsSubclassOf(typeof(Window)) &&
                t.GetInterfaces().Contains(typeof(ISetWindowProperties)));

            foreach (var type in matchingTypes)
            {
                Console.WriteLine(type.Name);
            }
        }

        public Window? FindWindow<T>() where T : Window
        {
            return Application.Current.Windows.OfType<T>().FirstOrDefault();
        }

        public void SetWindow<T>(WindowProperties properties) where T : Window, ISetWindowProperties
        {
            var window = FindWindow<T>();
            if (window == null)
            {
                window = this.serviceProvider.GetRequiredService<T>();
                window.Show();
            }
            ((ISetWindowProperties)window).SetWindowProperties(properties);
        }
    }
}
