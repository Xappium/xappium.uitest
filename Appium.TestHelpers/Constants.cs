using System;

namespace Appium.TestHelpers
{
    public static class Constants
    {
        public static readonly Uri DefaultAppiumServer = new Uri("http://127.0.0.1:4723/wd/hub");

        public const string UITestAppiumServer = "UITEST_APPIUMSERVER";

        public const string UITestDeviceName = "UITEST_DEVICENAME";

        public const string UITestOSVersion = "UITEST_OSVERSION";

        public const string UITestUDID = "UITEST_UDID";

        public const string UITestPlatform = "UITEST_PLATFORM";

        public const string UITestAppPath = "UITEST_APP_PATH";

        public const string UITestAppId = "UITEST_APPID";

        public const string UITestScreenshotPath = "UITEST_SCREENSHOT_PATH";

        public const string UITestAdditionalCapabilities = "UITEST_ADDITIONAL_CAPABILITIES";

        public const string UITestConfigFile = "uitest.json";
    }
}
