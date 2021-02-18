using OpenQA.Selenium.Appium;
using System.Drawing;

namespace Xappium.UITest
{
    internal class UIElement : IUIElement
    {
        private AppiumWebElement _element { get; }

        public UIElement(AppiumWebElement element)
        {
            _element = element;
        }

        public bool Displayed => _element.Displayed;

        public bool Enabled => _element.Enabled;

        public bool Selected => _element.Selected;

        public string Text => _element.Text;

        public Point Location => _element.Location;

        public Size Size => _element.Size;

        public Rectangle Rect => _element.Rect;
    }
}
