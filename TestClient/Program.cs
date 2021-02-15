using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using TestClient.Android;
using TestClient.Apple;

namespace TestClient
{
    class Program
    {
        const string ConfigPath = "uitest.json";

        static void Main(string[] args)
        {
            Console.WriteLine($"Current Directory: {Directory.GetCurrentDirectory()}");
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                AllowTrailingCommas = true,
                IgnoreNullValues = true,
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip
            };

            var binDir = new DirectoryInfo(Path.Combine(Directory.GetCurrentDirectory(), "bin"));
            if (!binDir.Exists)
                throw new DirectoryNotFoundException("Could not locate the bin directory");

            string appPath = default;
            string platform = null;
            if (binDir.GetFiles().Any(x => x.Extension == ".apk"))
            {
                appPath = binDir.GetFiles().First(x => x.Name.EndsWith("-Signed.apk")).FullName;
                platform = "Android";
            }
            else if (binDir.GetDirectories().Any(x => x.Name.EndsWith(".app")))
            {
                appPath = binDir.GetDirectories().First(x => x.Name.EndsWith(".app")).FullName;
                platform = "iOS";
            }
            else
            {
                var files = new[]
                {
                    Directory.EnumerateDirectories(Directory.GetCurrentDirectory()),
                    Directory.EnumerateFiles(Directory.GetCurrentDirectory())
                }.SelectMany(x => x);
                foreach (var file in files)
                    Console.WriteLine(file);

                throw new FileNotFoundException("Could not locate iOS .app or Android apk.");
            }

            var config = new TestConfiguration();
            if(File.Exists(ConfigPath))
            {
                config = JsonSerializer.Deserialize<TestConfiguration>(File.ReadAllText(ConfigPath), options);
            }

            config.Platform = platform;
            config.AppPath = appPath;

            switch (platform.ToLower())
            {
                case "ios":
                    AppleSimulator.ShutdownAllSimulators();
                    var device = AppleSimulator.GetSimulator();
                    if (device is null)
                        throw new NullReferenceException("Unable to locate the Device");

                    config.DeviceName = device.Name;
                    config.UDID = device.Udid;
                    config.OSVersion = device.OSVersion;
                    break;
                case "android":
                case "droid":
                    if (!Adb.DeviceIsConnected())
                    {
                        var sdkVersion = 29;
                        // Ensure SDK Installed
                        SdkManager.EnsureSdkIsInstalled(sdkVersion);

                        // Ensure Emulator Exists
                        if(!Emulator.ListEmulators().Any(x => x == AvdManager.DefaultUITestEmulatorName))
                            AvdManager.InstallEmulator(sdkVersion);

                        // Start Emulator
                        Emulator.StartEmulator(AvdManager.DefaultUITestEmulatorName);
                    }

                    var emulator = Adb.ListDevices().First();
                    config.DeviceName = emulator.Name;
                    config.UDID = emulator.Id;
                    config.OSVersion = $"{emulator.SdkVersion}";
                    break;
                default:
                    throw new PlatformNotSupportedException($"The Platform '{platform}' is not supported.");
            }

            File.WriteAllText(ConfigPath, JsonSerializer.Serialize(config, options));
        }
    }
}
