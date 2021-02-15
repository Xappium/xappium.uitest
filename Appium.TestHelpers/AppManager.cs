using System;
using System.IO;
using System.Reflection;
using Appium.TestHelpers.Configuration;

namespace Appium.TestHelpers
{
    public static class AppManager
    {
        private static readonly Lazy<Platform> _platform =
            new Lazy<Platform>(() => (Platform)Enum.Parse(typeof(Platform), Environment.GetEnvironmentVariable(Constants.UITestPlatform)));
        public static Platform Platform => _platform.Value;

        static ITestEngine engine;
        public static ITestEngine Engine =>
            engine ?? throw new NullReferenceException("'AppManager.App' not set. Call 'AppManager.StartApp()' before trying to access it.");

        public static void StartApp()
        {
            var callingAssembly = Assembly.GetCallingAssembly();
            ConfigurationLoader.Load(callingAssembly);
            var config = ConfigurationLoader.Current;

            if (string.IsNullOrEmpty(config.ScreenshotsPath))
                config.ScreenshotsPath = "Screenshots";

            if (config.AppiumServer is null)
                config.AppiumServer = Constants.DefaultAppiumServer;

            if (config.Platform != Platform.Android && config.Platform != Platform.iOS)
                config.Platform = Platform.iOS;

            if (!Directory.Exists(config.ScreenshotsPath))
                Directory.CreateDirectory(config.ScreenshotsPath);

            if ((!Directory.Exists(config.AppPath) && config.Platform == Platform.iOS) ||
                (!File.Exists(config.AppPath) && config.Platform == Platform.Android))
                throw new FileNotFoundException($"The App Package could not be found at the specified location. '{config.AppPath}'");

            //app = config.Platform switch
            //{
            //    Platform.Android => new AndroidApp(config),
            //    Platform.iOS => new iOSApp(config),
            //    _ => throw new PlatformNotSupportedException()
            //};
            engine = new AppiumTestEngine(config);

            //app.InstallApp(config.AppPath);
        }
    }
}
