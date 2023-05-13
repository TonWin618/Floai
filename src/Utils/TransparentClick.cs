using System;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace Floai.Utils
{
    static class TransparentClick
    {
        //Mouse click penetration of WPF transparent control
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int GWL_EXSTYLE = -20;
        [DllImport("user32", EntryPoint = "SetWindowLong")]
        private static extern uint SetWindowLong(IntPtr hwnd, int nIndex, uint dwNewLong);
        [DllImport("user32", EntryPoint = "GetWindowLong")]
        private static extern uint GetWindowLong(IntPtr hwnd, int nIndex);

        public static void Enable(System.Windows.Window window)
        {
            IntPtr hwnd = new WindowInteropHelper(window).Handle;
            uint extendedStyle = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, extendedStyle | WS_EX_TRANSPARENT);
        }
    }
}
