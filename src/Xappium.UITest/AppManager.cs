using System;
using System.IO;
using System.Reflection;
using Xappium.UITest.Configuration;
using Xappium.UITest.Platforms;

namespace Xappium.UITest
{
    public static class AppManager
    {
        static UITestConfiguration _testConfig;
        public static Platform Platform =>
            _testConfig is null || _testConfig.Platform == Platform.NotSet ?
            throw new NullReferenceException($"'{nameof(AppManager)}.{nameof(Platform)}' not set. Call '{nameof(AppManager)}.{nameof(StartApp)}()' before trying to access it.") : _testConfig.Platform;

        static ITestEngine _engine;
        public static ITestEngine Engine =>
            _engine ?? throw new NullReferenceException($"'{nameof(AppManager)}.{nameof(Engine)}' not set. Call '{nameof(AppManager)}.{nameof(StartApp)}()' before trying to access it.");

        public static void StartApp()
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            ConfigurationLoader.Load(callingAssembly);
            var config = ConfigurationLoader.Current;

            StartApp(config);
        }

        public static void StartApp(UITestConfiguration testConfig)
        {
            if (testConfig is null)
                throw new ArgumentNullException(nameof(testConfig));

            _testConfig = testConfig;

            if (string.IsNullOrEmpty(testConfig.ScreenshotsPath))
                testConfig.ScreenshotsPath = "Screenshots";

            if (testConfig.AppiumServer is null)
                testConfig.AppiumServer = UITestDefaults.DefaultAppiumServer;

            if (testConfig.Platform != Platform.Android && testConfig.Platform != Platform.iOS)
                testConfig.Platform = Platform.iOS;

            if (!Directory.Exists(testConfig.ScreenshotsPath))
                Directory.CreateDirectory(testConfig.ScreenshotsPath);

            if ((!Directory.Exists(testConfig.AppPath) && testConfig.Platform == Platform.iOS) ||
                (!File.Exists(testConfig.AppPath) && testConfig.Platform == Platform.Android))
                throw new FileNotFoundException($"The App Package could not be found at the specified location. '{testConfig.AppPath}'");

            _engine = testConfig.Platform switch
            {
                Platform.NotSet => throw new NotSupportedException("The platform has not been set in the test config. You must select a valid platform"),
                Platform.Android => new AndroidXappiumTestEngine(testConfig),
                Platform.iOS => new iOSXappiumTestEngine(testConfig),
                _ => throw new PlatformNotSupportedException($"The selected platform {testConfig.Platform} is not currently implemented"),
            };
        }

        internal static void AppStopped()
        {
            _testConfig = null;
            _engine = null;
        }
    }
}
