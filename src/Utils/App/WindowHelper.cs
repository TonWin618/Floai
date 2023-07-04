using System.Linq;
using System.Windows;

namespace Floai.Utils.App
{
    public static class WindowHelper
    {
        public static Window? FindiWindow <T>() where T : Window
        {
            return Application.Current.Windows.OfType<T>().FirstOrDefault();
        }
    }
}
