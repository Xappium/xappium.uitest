using System;
using System.Diagnostics;
using static TestClient.Android.AndroidTool;

namespace TestClient.Android
{
    public static class AvdManager
    {
        public static readonly string ToolPath = LocateUtility("avdmanager");

        public const string DefaultUITestEmulatorName = "uitest_android_emulator";

        public static void InstallEmulator(int sdkVersion = 29)
        {
            ThrowIfNull(ToolPath, nameof(AvdManager));

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                throw new PlatformNotSupportedException("Installing the Enumlator is not supported on Windows");

            Console.WriteLine($"Installing Emulator for SDK Version: {sdkVersion}");

            using var process = new Process
            {
                StartInfo = new ProcessStartInfo(ToolPath, $"create avd -n {DefaultUITestEmulatorName} -k 'system-images;android-{sdkVersion};google_apis;x86' --force")
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };

            // Echo no

            process.Start();
            while (!process.StandardOutput.EndOfStream)
                Console.WriteLine(process.StandardOutput.ReadLine());
        }
    }
}
