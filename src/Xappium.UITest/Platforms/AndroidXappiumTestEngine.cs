using System;
using System.IO;
using Xappium.UITest.Configuration;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.Android;
using System.Drawing;
using OpenQA.Selenium.Interactions.Internal;
using OpenQA.Selenium.Interactions;
using Xappium.UITest.Extensions;

namespace Xappium.UITest.Platforms
{
    internal class AndroidXappiumTestEngine : XappiumTestEngineBase<AndroidDriver<AndroidElement>, AndroidElement>
    {
        public AndroidXappiumTestEngine(UITestConfiguration config)
            : base(config)
        {
        }

        protected override AndroidDriver<AndroidElement> CreateDriver(AppiumOptions options, UITestConfiguration config)
        {
            if (!string.IsNullOrWhiteSpace(config.AppPath))
            {
                var apkFileInfo = new FileInfo(config.AppPath);
                if (apkFileInfo.Exists)
                    options.AddAdditionalCapability(MobileCapabilityType.App, apkFileInfo.FullName);
            }

            AddAdditionalCapability(options, MobileCapabilityType.PlatformName, "Android");
            AddAdditionalCapability(options, MobileCapabilityType.DeviceName, config.DeviceName);
            AddAdditionalCapability(options, MobileCapabilityType.AutomationName, "Espresso");
            AddAdditionalCapability(options, "showGradleLog", true);

            if(!string.IsNullOrEmpty(config.UDID))
            {
                AddAdditionalCapability(options, MobileCapabilityType.Udid, config.UDID);
            }

            // Espresso build config is supposed to be ok with a json string
            // but the dotnet driver doesn't seem to work with this, and it must instead be
            // a file path to a json file.
            // This will check if the config option is a json string and create a temp file
            // to pass instead if so
            options.FixEspressoBuildConfigOptions();
            
            return new AndroidDriver<AndroidElement>(config.AppiumServer, options, TimeSpan.FromSeconds(90));
        }

        protected override IUIElement CreateUIElement(AndroidElement nativeElement) =>
            new AndroidUIElement(nativeElement);

        private class AndroidUIElement : UIElementBase<AndroidElement>
        {
            public AndroidUIElement(AndroidElement element)
                : base(element)
            {
            }

            public override ICoordinates Coordinates => _element.Coordinates;

            public override Rectangle Rect => _element.Rect;

            public override Point LocationOnScreenOnceScrolledIntoView => _element.LocationOnScreenOnceScrolledIntoView;
        }
    }
}
