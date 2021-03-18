using System;
using Xappium.UITest.Configuration;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.Enums;
using System.Drawing;
using OpenQA.Selenium.Interactions.Internal;

namespace Xappium.UITest.Platforms
{
    internal class iOSXappiumTestEngine : XappiumTestEngineBase<IOSDriver<IOSElement>, IOSElement>
    {
        public iOSXappiumTestEngine(UITestConfiguration config)
            : base(config)
        {
        }

        protected override IOSDriver<IOSElement> CreateDriver(AppiumOptions options, UITestConfiguration config)
        {
            // The value of DEVICE_NAME is only used for running on the iOS simulator,
            // but must also have some (any) value for iOS and Android physical devices
            AddAdditionalCapability(options, MobileCapabilityType.DeviceName, config.DeviceName);
            if (!string.IsNullOrWhiteSpace(config.AppPath))
                AddAdditionalCapability(options, MobileCapabilityType.App, config.AppPath);
            AddAdditionalCapability(options, MobileCapabilityType.PlatformName, "iOS");
            AddAdditionalCapability(options, MobileCapabilityType.PlatformVersion, config.OSVersion);
            AddAdditionalCapability(options, MobileCapabilityType.AutomationName, "XCUITest");
            AddAdditionalCapability(options, "autoAcceptAlerts", true);
            //AddAdditionalCapability(options, "isHeadless", true);
            AddAdditionalCapability(options, IOSMobileCapabilityType.LaunchTimeout, 60000);
            AddAdditionalCapability(options, "appium:screenshotQuality", 2);
            AddAdditionalCapability(options, "appium:showIOSLog", true);

            // https://github.com/appium/appium/issues/12094#issuecomment-482593020
            AddAdditionalCapability(options, "wdaStartupRetries", "4");
            AddAdditionalCapability(options, "iosInstallPause","8000" );
            AddAdditionalCapability(options, "wdaStartupRetryInterval", "20000");

            return new IOSDriver<IOSElement>(config.AppiumServer, options);
        }

        protected override IUIElement CreateUIElement(IOSElement nativeElement) =>
            new iOSUIElement(nativeElement);

        private class iOSUIElement : UIElementBase<IOSElement>
        {
            public iOSUIElement(IOSElement element)
                : base(element)
            {
            }

            public override Rectangle Rect => _element.Rect;

            public override ICoordinates Coordinates => _element.Coordinates;

            public override Point LocationOnScreenOnceScrolledIntoView => _element.LocationOnScreenOnceScrolledIntoView;
        }
    }
}
