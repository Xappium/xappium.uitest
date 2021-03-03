using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Xappium.Android;
using Xappium.Apple;
using Xappium.BuildSystem;
using Xappium.Tools;

namespace Xappium
{
    internal class Program
    {
        private const string ConfigFileName = "uitest.json";

        private int sdkVersion = 29;

        private FileInfo UITestProjectPathInfo => string.IsNullOrEmpty(UITestProjectPath) ? null : new FileInfo(UITestProjectPath);

        private FileInfo DeviceProjectPathInfo => string.IsNullOrEmpty(DeviceProjectPath) ? null : new FileInfo(DeviceProjectPath);

        public static Task<int> Main(string[] args)
            => CommandLineApplication.ExecuteAsync<Program>(args);

        [Option(Description = "Specifies the csproj path of the UI Test project",
            LongName = "uitest-project-path",
            ShortName = "uitest")]
        public string UITestProjectPath { get; }

        [Option(Description = "Specifies the Head Project csproj path for your iOS or Android project.",
            LongName = "app-project-path",
            ShortName = "app")]
        public string DeviceProjectPath { get; }

        [Option(Description = "Specifies the target platform such as iOS or Android.",
            LongName = "platform",
            ShortName = "p")]
        public string Platform { get; }

        [Option(Description = "Specifies the Build Configuration to use on the Platform head and UITest project",
            LongName = "configuration",
            ShortName = "c")]
        public string Configuration { get; } = "Release";

        [Option(Description = "Specifies a UITest.json configuration path that overrides what may be in the UITest project build output directory",
            LongName = "uitest-configuration",
            ShortName = "ui-config")]
        public string ConfigurationPath { get; }

        [Option(Description = "Specifies the test artifact folder",
            LongName = "artifact-staging-directory",
            ShortName = "artifacts")]
        public string BaseWorkingDirectory { get; } = Path.Combine(Environment.CurrentDirectory, "UITest");

        [Option(Description = "Will write the generated uitest.json to the console. This should only be done if you do not have sensative settings that may be written to the console.",
            LongName = "show-config",
            ShortName = "show")]
        public bool DisplayGeneratedConfig { get; }

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members",
            Justification = "Called by McMaster")]
        private async Task<int> OnExecuteAsync(CancellationToken cancellationToken)
        {
            IDisposable appium = null;
            try
            {
                if (!Node.IsInstalled)
                    throw new Exception("Your environment does not appear to have Node installed. This is required to run Appium");

                if (Directory.Exists(BaseWorkingDirectory))
                    Directory.Delete(BaseWorkingDirectory, true);

                Directory.CreateDirectory(BaseWorkingDirectory);

                Console.WriteLine($"Build and Test artifacts will be stored at {BaseWorkingDirectory}");

                ValidatePaths();

                var headBin = Path.Combine(BaseWorkingDirectory, "bin", "device");
                var uiTestBin = Path.Combine(BaseWorkingDirectory, "bin", "uitest");

                Directory.CreateDirectory(headBin);
                Directory.CreateDirectory(uiTestBin);

                // HACK: The iOS SDK will mess up the generated app output if a Separator is not at the end of the path.
                headBin += Path.DirectorySeparatorChar;
                uiTestBin += Path.DirectorySeparatorChar;

                var appProject = CSProjFile.Load(DeviceProjectPathInfo, new DirectoryInfo(headBin), Platform);
                if (!await appProject.IsSupported())
                    throw new PlatformNotSupportedException($"{appProject.Platform} is not supported on this machine. Please check that you have the correct build dependencies.");

                await appProject.Build(Configuration, cancellationToken).ConfigureAwait(false);

                var uitestProj = CSProjFile.Load(UITestProjectPathInfo, new DirectoryInfo(uiTestBin), string.Empty);
                await uitestProj.Build(Configuration, cancellationToken).ConfigureAwait(false);

                if (cancellationToken.IsCancellationRequested)
                    return 0;

                if (appProject is DotNetMauiProjectFile && appProject.Platform == "Android")
                    sdkVersion = 30;

                GenerateTestConfig(headBin, uiTestBin, appProject.Platform);

                if(!await Appium.Install(cancellationToken))
                {
                    return 0;
                }

                appium = await Appium.Run(BaseWorkingDirectory).ConfigureAwait(false);

                await DotNetTool.Test(UITestProjectPathInfo.FullName, uiTestBin, Configuration?.Trim(), Path.Combine(BaseWorkingDirectory, "results"), cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine(ex);
                Console.ResetColor();

                var logsDir = new DirectoryInfo(Path.Combine(BaseWorkingDirectory, "logs"));
                if(!logsDir.Exists)
                    logsDir.Create();
                File.WriteAllText(Path.Combine(logsDir.FullName, "crash.log"), ex.ToString());
                return 1;
            }
            finally
            {
                appium?.Dispose();

                var binDir = Path.Combine(BaseWorkingDirectory, "bin");
                if(Directory.Exists(binDir))
                    Directory.Delete(binDir, true);
            }

            return 0;
        }

        private void ValidatePaths()
        {
            if (UITestProjectPathInfo.Extension != ".csproj")
                throw new InvalidOperationException($"The path '{UITestProjectPath}' does not specify a valid csproj");
            else if (DeviceProjectPathInfo.Extension != ".csproj")
                throw new InvalidOperationException($"The path '{DeviceProjectPath}' does not specify a valid csproj");
            else if (!UITestProjectPathInfo.Exists)
                throw new FileNotFoundException($"The specified UI Test project path does not exist: '{UITestProjectPath}'");
            else if (!DeviceProjectPathInfo.Exists)
                throw new FileNotFoundException($"The specified Platform head project path does not exist: '{DeviceProjectPath}'");
        }

        private void GenerateTestConfig(string headBin, string uiTestBin, string platform)
        {
            var binDir = new DirectoryInfo(headBin);
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                AllowTrailingCommas = true,
                IgnoreNullValues = true,
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var appPath = platform switch
            {
                "Android" => binDir.GetFiles().First(x => x.Name.EndsWith("-Signed.apk")).FullName,
                "iOS" => binDir.GetDirectories().First(x => x.Name.EndsWith(".app")).FullName,
                null => throw new ArgumentNullException("No platform was specified"),
                _ => throw new PlatformNotSupportedException($"The {platform} is not supported")
            };

            var config = new TestConfiguration();
            var testConfig = Path.Combine(uiTestBin, ConfigFileName);
            if (!string.IsNullOrEmpty(ConfigurationPath))
            {
                if (!File.Exists(ConfigurationPath))
                    throw new FileNotFoundException($"Could not locate the specified uitest configuration at: '{ConfigurationPath}'");
                config = JsonSerializer.Deserialize<TestConfiguration>(File.ReadAllText(ConfigurationPath), options);
            }
            else if(File.Exists(testConfig))
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
                config.ScreenshotsPath = Path.Combine(BaseWorkingDirectory, "screenshots");

            switch(platform)
            {
                case "Android":
                    // Ensure WebDrivers are installed
                    SdkManager.InstallWebDriver();

                    // Check for connected device
                    if (!Adb.DeviceIsConnected())
                    {
                        // Ensure SDK Installed
                        SdkManager.EnsureSdkIsInstalled(sdkVersion);

                        // Ensure Emulator Exists
                        if (!Emulator.ListEmulators().Any(x => x == AvdManager.DefaultUITestEmulatorName))
                            AvdManager.InstallEmulator(sdkVersion);

                        // Start Emulator
                        Emulator.StartEmulator(AvdManager.DefaultUITestEmulatorName);
                    }

                    var emulator = Adb.ListDevices().First();
                    config.DeviceName = emulator.Name;
                    config.UDID = emulator.Id;
                    config.OSVersion = $"{emulator.SdkVersion}";
                    break;
                case "iOS":
                    AppleSimulator.ShutdownAllSimulators();
                    var device = AppleSimulator.GetSimulator();
                    if (device is null)
                        throw new NullReferenceException("Unable to locate the Device");

                    config.DeviceName = device.Name;
                    config.UDID = device.Udid;
                    config.OSVersion = device.OSVersion;
                    break;
            }

            var jsonOutput = JsonSerializer.Serialize(config, options);
            File.WriteAllText(testConfig, jsonOutput);

            if(DisplayGeneratedConfig)
                Console.WriteLine(jsonOutput);
        }
    }
}
