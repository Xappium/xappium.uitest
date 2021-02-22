using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Xappium.Apple
{
    internal static class AppleSimulator
    {
        private const string AppleSimulatorRuntimeKey = "com.apple.CoreSimulator.SimRuntime.iOS-";

        public static void ShutdownAllSimulators()
        {
            Console.WriteLine("Shutting down simulators");
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo("xcrun", "simctl shutdown all")
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                // don't need to do anything....
            }
        }

        public static IEnumerable<AppleDeviceInfo> GetAvailableSimulators()
        {
            Console.WriteLine("Getting Available Simulators");
            var results = new Dictionary<int, string>();
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo("xcrun", "simctl list devices available --json")
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();

            var sb = new StringBuilder();
            while(!process.StandardOutput.EndOfStream)
            {
                sb.Append(process.StandardOutput.ReadLine());
            }

            var json = sb.ToString();
            var deviceList = JsonSerializer.Deserialize<AppleDeviceList>(json);

            return deviceList.Devices
                .Where(x => x.Key.StartsWith(AppleSimulatorRuntimeKey))
                .SelectMany(x =>
                {
                    var osVersion = x.Key.Replace(AppleSimulatorRuntimeKey, string.Empty).Replace("-", ".");

                    foreach (var s in x.Value)
                        s.OSVersion = osVersion;
                    return x.Value;
                })
                .Where(x => x.IsAvailable);
        }

        public static AppleDeviceInfo GetSimulator()
        {
            var devices = GetAvailableSimulators();
            Console.WriteLine("Getting Default iPhone Simulator");
            return devices
                .Where(x => !x.Name.Contains("Max") && Regex.IsMatch(x.Name, @"^iPhone \d\d Pro") && x.IsAvailable)
                .OrderByDescending(x => x.OSVersion)
                .ThenByDescending(x => x.Name)
                .FirstOrDefault();
        }
    }
}
