using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using static Xappium.Client.Android.AndroidTool;

namespace Xappium.Client.Android
{
    internal static class Adb
    {
        private const string androidDeviceRegex = @"\s+(device)$";

        public static readonly string ToolPath = LocateUtility("adb");

        public static bool DeviceIsConnected()
        {
            ThrowIfNull(ToolPath, nameof(Adb));
            try
            {
                var devices = ListDevices();
                var device = devices.FirstOrDefault();
                return device != null;
            }
            catch
            {
                return false;
            }
        }

        public static IEnumerable<AndroidDevice> ListDevices()
        {
            ThrowIfNull(ToolPath, nameof(Adb));
            var result = ProcessHelper.Run(ToolPath, "devices");
            if (result.IsErred)
                return Array.Empty<AndroidDevice>();

            var ids = result.Output.Where(x => Regex.IsMatch(x, androidDeviceRegex))
                .Select(x => Regex.Replace(x, androidDeviceRegex, string.Empty));

            // List of devices attached
            var devices = new List<AndroidDevice>();
            foreach (var deviceId in ids)
            {
                result = ProcessHelper.Run(ToolPath, $"-s {deviceId} shell getprop");
                if (result.IsErred)
                    continue;

                devices.Add(new AndroidDevice(deviceId, result.Output.ToArray()));
            }

            if (!devices.Any())
                Console.WriteLine("-- NO DEVICES FOUND --");

            return devices;
        }
    }
}
