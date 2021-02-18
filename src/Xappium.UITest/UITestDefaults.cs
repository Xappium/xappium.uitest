using System;

namespace Xappium.UITest
{
    public static class UITestDefaults
    {
        public static readonly Uri DefaultAppiumServer = new Uri("http://127.0.0.1:4723/wd/hub");

        public const string UITestConfigFile = "uitest.json";
    }
}
