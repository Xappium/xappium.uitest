using System.Drawing;

namespace Xappium.UITest
{
    public interface IUIElement
    {
        bool Displayed { get; }

        bool Enabled { get; }

        bool Selected { get; }

        string Text { get; }

        Point Location { get; }

        Size Size { get; }

        Rectangle Rect { get; }
    }
}
