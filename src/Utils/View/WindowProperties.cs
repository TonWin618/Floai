using System.Windows;

namespace Floai.Utils.View;

public record WindowProperties
{
    public double Top { get; private set; }
    public double Left { get; private set; }
    public double Width { get; private set; }
    public double Height { get; private set; }
    public double Bottom { get; private set; }
    public double Right { get; private set; }

    public WindowProperties(Window window)
    {
        Top = window.Top;
        Left = window.Left;
        Width = window.Width;
        Height = window.Height;

        Bottom = Top + Height;
        Right = Left + Width;
    }
}
