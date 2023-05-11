using System.Linq;
using System.Windows;

namespace Floai.Utils
{
    public static class WindowHelper
    {
        public static bool IsWindowOpen<T>() where T : Window
        {
            return Application.Current.Windows.OfType<T>().Any();
        }
    }
}
