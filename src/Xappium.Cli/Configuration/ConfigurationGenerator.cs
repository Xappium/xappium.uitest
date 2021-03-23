using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Xappium.Android;
using Xappium.Apple;
using Xappium.Logging;
using Xappium.Tools;

namespace Xappium.Configuration
{
    internal static class ConfigurationGenerator
    {
        private const string ConfigFileName = "uitest.json";
        private static readonly JsonSerializerOptions options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            AllowTrailingCommas = true,
            IgnoreNullValues = true,
            PropertyNameCaseInsensitive = true,
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        public async static Task GenerateTestConfig(string headBin, string uiTestBin, string platform, string configurationPath, string baseWorkingDirectory, int? androidSdk, bool displayGeneratedConfig, CancellationToken cancellationToken)
        {
            var binDir = new DirectoryInfo(headBin);

            var appPath = GetAppPath(platform, binDir);

            var testConfig = Path.Combine(uiTestBin, ConfigFileName);
            var config = BuildBaseConfiguration(uiTestBin, ConfigFileName, configurationPath, testConfig, options, platform, appPath, baseWorkingDirectory);

            switch (platform)
            {
                case "Android":
                    await ConfigureForAndroidTests(config, androidSdk, headBin, cancellationToken);
                    break;
                case "iOS":
                    await ConfigureForIOSTests(config, cancellationToken);
                    break;
            }

            var jsonOutput = JsonSerializer.Serialize(config, options);
            File.WriteAllText(testConfig, jsonOutput);

            if (displayGeneratedConfig)
                Logger.WriteLine(jsonOutput, LogLevel.Normal);
        }

        private static TestConfiguration BuildBaseConfiguration(string uiTestBin, string configFileName, string configurationPath, string testConfig, JsonSerializerOptions options, string platform, string appPath, string baseWorkingDirectory)
        {
            var config = new TestConfiguration();
            if (!string.IsNullOrEmpty(configurationPath))
            {
                if (!File.Exists(configurationPath))
                    throw new FileNotFoundException($"Could not locate the specified uitest configuration at: '{configurationPath}'");
                config = JsonSerializer.Deserialize<TestConfiguration>(File.ReadAllText(configurationPath), options);
            }
            else if (File.Exists(testConfig))
            {
                config = JsonSerializer.Deserialize<TestConfiguration>(File.ReadAllText(testConfig), options);
            }

            if (config.Capabilities is null)
                config.Capabilities = new Dictionary<string, string>();

            if (config.Settings is null)
                config.Settings = new Dictionary<string, string>();

            config.Platform = platform;
            config.AppPath = appPath;

            if (string.IsNullOrEmpty(config.ScreenshotsPath))
                config.ScreenshotsPath = Path.Combine(baseWorkingDirectory, "screenshots");

            return config;
        }

        private static async Task ConfigureForIOSTests(TestConfiguration config, CancellationToken cancellationToken)
        {
            // Install Helpers for testing on iOS Devices / Simulators
            // await Pip.UpgradePip(cancellationToken).ConfigureAwait(false);
            // await Gem.InstallXcPretty(cancellationToken).ConfigureAwait(false);
            // await Brew.InstallAppleSimUtils(cancellationToken).ConfigureAwait(false);
            // await Brew.InstallFFMPEG(cancellationToken).ConfigureAwait(false);
            // await Brew.InstallIdbCompanion(cancellationToken).ConfigureAwait(false);
            // await Pip.InstallIdbClient(cancellationToken).ConfigureAwait(false);
            await Task.CompletedTask;

            if (cancellationToken.IsCancellationRequested)
                return;

            var device = AppleSimulator.GetSimulator();
            if (device is null)
                throw new NullReferenceException("Unable to locate the Device");

            config.DeviceName = device.Name;
            config.UDID = device.Udid;
            config.OSVersion = device.OSVersion;
            AppleSimulator.ShutdownAllSimulators();
        }

        private static async Task ConfigureForAndroidTests(TestConfiguration config, int? androidSdk, string headBin, CancellationToken cancellationToken)
        {
            // Ensure WebDrivers are installed
            await SdkManager.InstallWebDriver(cancellationToken).ConfigureAwait(false);

            // Ensure latest CmdLine tools are installed
            await SdkManager.InstallLatestCommandLineTools(cancellationToken).ConfigureAwait(false);

            var sdkVersion = ApkHelper.GetAndroidSdkVersion(androidSdk, headBin);
            Logger.WriteLine($"Targeting Android Sdk: {sdkVersion}", LogLevel.Minimal);
            var appActivity = ApkHelper.GetAppActivity(headBin);

            if(!config.Capabilities.ContainsKey("appActivity"))
                config.Capabilities.Add("appActivity", appActivity);

            var emulatorName = $"{AvdManager.DefaultUITestEmulatorName}{sdkVersion}";
            // Check for connected device
            if (await Adb.DeviceIsConnected(cancellationToken))
            {
                var androidDevice = (await Adb.ListDevices(cancellationToken).ConfigureAwait(false)).First();
                config.DeviceName = androidDevice.Name;
                config.UDID = androidDevice.Id;
                config.OSVersion = $"{androidDevice.SdkVersion}";
            }
            else
            {
                // Ensure SDK Installed
                await SdkManager.EnsureSdkIsInstalled(sdkVersion, cancellationToken).ConfigureAwait(false);

                // Ensure Emulator Exists
                if (!(await Emulator.ListEmulators(cancellationToken)).Any(x => x == emulatorName))
                    await AvdManager.InstallEmulator(emulatorName, sdkVersion, cancellationToken);

                // Let Appium Start and control the Emulator
                config.DeviceName = emulatorName;
                config.OSVersion = $"{sdkVersion}";

                if(!config.Capabilities.ContainsKey("avd"))
                    config.Capabilities.Add("avd", emulatorName);
            }

        }

        private static string GetAppPath(string platform, DirectoryInfo binDir)
        {
            return platform switch
            {
                "Android" => binDir.GetFiles().First(x => x.Name.EndsWith("-Signed.apk")).FullName,
                "iOS" => binDir.GetDirectories().First(x => x.Name.EndsWith(".app")).FullName,
                null => throw new ArgumentNullException("No platform was specified"),
                _ => throw new PlatformNotSupportedException($"The {platform} is not supported")
            };
        }
    }
}
