using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Xappium.UITest.Extensions;
using Newtonsoft.Json;

namespace Xappium.UITest.Configuration
{
    internal static class ConfigurationLoader
    {
        private static UITestConfiguration _current;
        public static UITestConfiguration Current => _current ?? throw new NullReferenceException("The UITestConfiguratiion must be initialized");

        public static UITestConfiguration Load(Assembly assembly)
        {
            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                throw new PlatformNotSupportedException("UI Tests are currently only supported on macOS only.");

            var config = new UITestConfiguration();

            if (File.Exists(UITestDefaults.UITestConfigFile))
            {
                config = JsonConvert.DeserializeObject<UITestConfiguration>(File.ReadAllText(UITestDefaults.UITestConfigFile));
            }

            var resourceId = assembly.GetManifestResourceNames().FirstOrDefault(x => x.EndsWith(UITestDefaults.UITestConfigFile));
            if (!string.IsNullOrEmpty(resourceId))
            {
                using var stream = assembly.GetManifestResourceStream(resourceId);
                config.Merge(stream);
            }

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Constants.UITestPlatform)) &&
                Enum.TryParse<Platform>(Environment.GetEnvironmentVariable(Constants.UITestPlatform), out var envPlatform) &&
                (envPlatform == Platform.Android || envPlatform == Platform.iOS))
                config.Platform = envPlatform;

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Constants.UITestAppPath)))
                config.AppPath = Environment.GetEnvironmentVariable(Constants.UITestAppPath);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Constants.UITestAppId)))
                config.AppId = Environment.GetEnvironmentVariable(Constants.UITestAppId);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Constants.UITestDeviceName)))
                config.DeviceName = Environment.GetEnvironmentVariable(Constants.UITestDeviceName);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Constants.UITestUDID)))
                config.UDID = Environment.GetEnvironmentVariable(Constants.UITestUDID);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Constants.UITestOSVersion)))
                config.OSVersion = Environment.GetEnvironmentVariable(Constants.UITestOSVersion);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Constants.UITestScreenshotPath)))
                config.ScreenshotsPath = Environment.GetEnvironmentVariable(Constants.UITestScreenshotPath);

            if (!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Constants.UITestAppiumServer)) &&
                Uri.TryCreate(Environment.GetEnvironmentVariable(Constants.UITestAppiumServer), UriKind.Absolute, out var appiumServer))
                config.AppiumServer = appiumServer;

            if(!string.IsNullOrEmpty(Environment.GetEnvironmentVariable(Constants.UITestAdditionalCapabilities)))
            {
                var capabilities = Environment.GetEnvironmentVariable(Constants.UITestAdditionalCapabilities)
                    .Split(';')
                    .ToDictionary(x => x.Split('=')[0], x => x.Split('=')[1]);
                foreach ((var key, string value) in capabilities)
                    config.Capabilities[key] = value;
            }

            return _current = config;
        }
    }
}
