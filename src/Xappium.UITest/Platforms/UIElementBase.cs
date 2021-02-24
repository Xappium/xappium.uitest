using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions.Internal;

namespace Xappium.UITest.Platforms
{
    internal abstract class UIElementBase<T> : IUIElement
        where T : IWebElement
    {
        protected T _element { get; }

        public UIElementBase(T element)
        {
            _element = element;
        }

        public bool Displayed => _element.Displayed;

        public bool Enabled => _element.Enabled;

        public bool Selected => _element.Selected;

        public string Text => _element.Text;

        public Point Location => _element.Location;

        public Size Size => _element.Size;

        public abstract Rectangle Rect { get; }

        public abstract ICoordinates Coordinates { get; }

        public abstract Point LocationOnScreenOnceScrolledIntoView { get; }
    }
}
