using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using static Xappium.Client.Android.AndroidTool;

namespace Xappium.Client.Android
{
    internal static class AvdManager
    {
        public static readonly string ToolPath = LocateUtility("avdmanager");

        public const string DefaultUITestEmulatorName = "uitest_android_emulator";

        public static void InstallEmulator(int sdkVersion = 29)
        {
            ThrowIfNull(ToolPath, nameof(AvdManager));

            if (Environment.OSVersion.Platform == PlatformID.Win32NT)
                throw new PlatformNotSupportedException("Installing the Enumlator is not supported on Windows");

            var device = GetDevices()
                .Where(x => Regex.IsMatch(x, @"^pixel_\d$"))
                .OrderByDescending(x => x)
                .FirstOrDefault();

            Console.WriteLine($"Installing Emulator for SDK Version: {sdkVersion} with Device {device}");

            var result = ProcessHelper.Run(ToolPath, $"create avd -n {DefaultUITestEmulatorName} -k \"system-images;android-{sdkVersion};google_apis_playstore;x86\" --device {device} --force", displayRealtimeOutput: true, new[] { "no" });

            // Echo no

            if (result.IsErred)
                throw new Exception(result.Error);
        }

        public static void DeleteEmulator()
        {
            ThrowIfNull(ToolPath, nameof(AvdManager));

            var result = ProcessHelper.Run(ToolPath, $"delete avd -n {DefaultUITestEmulatorName}", displayRealtimeOutput: true);

            if (result.IsErred)
                throw new Exception(result.Error);
        }

        public static IEnumerable<string> GetDevices()
        {
            var result = ProcessHelper.Run(ToolPath, "list device -c");

            if (result.IsErred)
                throw new Exception(result.Error);

            // Skip the Parsing notification
            return result.Output.Skip(1);
        }
    }
}
