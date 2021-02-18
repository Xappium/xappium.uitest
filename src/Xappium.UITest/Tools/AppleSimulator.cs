using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace Xappium.UITest.Tools
{
    public static class AppleSimulator
    {
        private const string AppleSimulatorRuntimeKey = "com.apple.CoreSimulator.SimRuntime.iOS-";

        public static IEnumerable<AppleDeviceInfo> GetAvailableSimulators()
        {
            var results = new Dictionary<int, string>();
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("xcrun", "simctl list devices available --json")
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            process.WaitForExit();

            var json = process.StandardOutput.ReadToEnd();
            var options = new JsonSerializerOptions(JsonSerializerDefaults.Web)
            {
                AllowTrailingCommas = true,
                PropertyNameCaseInsensitive = true,
            };
            var deviceList = JsonSerializer.Deserialize<AppleDeviceList>(json, options);

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
            return GetAvailableSimulators()
                .Where(x => !x.Name.Contains("Max") && Regex.IsMatch(x.Name, @"^iPhone \d\d Pro") && x.IsAvailable)
                .OrderByDescending(x => x.OSVersion)
                .ThenByDescending(x => x.Name)
                .First();
        }
    }
}
