using System.Drawing;
using OpenQA.Selenium.Interactions.Internal;

namespace Xappium.UITest
{
    public interface IUIElement
    {
        ICoordinates Coordinates { get; }

        bool Displayed { get; }

        bool Enabled { get; }

        bool Selected { get; }

        string Text { get; }

        Point Location { get; }

        Point LocationOnScreenOnceScrolledIntoView { get; }

        Size Size { get; }

        Rectangle Rect { get; }
    }
}
