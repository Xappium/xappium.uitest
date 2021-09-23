using System;
using System.Drawing;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Interactions.Internal;

namespace Xappium.UITest.Platforms
{
    internal class UIElementBase<T> : IUIElement
        where T : IWebElement
    {
        protected T _element { get; }

        public UIElementBase(T element)
        {
            // If you're supporting a new IWebElement type, put a new case in each of the switch
            // statements.

            _element = element switch
            {
                // this is a fail fast check to get your attention if
                // a new type gets added
                AndroidElement _ => element,
                IOSElement _ => element,
                _ => throw UnknownElementType()
            };
        }

        static NotImplementedException UnknownElementType() => new NotImplementedException($"Unknown element type {typeof (T).FullName}");

        public bool Displayed => _element.Displayed;

        public bool Enabled => _element.Enabled;

        public bool Selected => _element.Selected;

        public string Text => _element.Text;

        public Point Location => _element.Location;

        public Size Size => _element.Size;


        public Rectangle Rect =>
            _element switch
            {
                AndroidElement android => android.Rect,
                IOSElement ios => ios.Rect,
                _ => throw UnknownElementType ()
            };

        public ICoordinates Coordinates =>
            _element switch
            {
                AndroidElement android => android.Coordinates,
                IOSElement ios => ios.Coordinates,
                _ => throw UnknownElementType()
            };

        public Point LocationOnScreenOnceScrolledIntoView =>
            _element switch
            {
                AndroidElement android => android.LocationOnScreenOnceScrolledIntoView,
                IOSElement ios => ios.LocationOnScreenOnceScrolledIntoView,
                _ => throw UnknownElementType()
            };
    }
}
