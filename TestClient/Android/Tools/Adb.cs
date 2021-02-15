using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using static TestClient.Android.AndroidTool;

namespace TestClient.Android
{
    public static class Adb
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
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo(ToolPath, "devices")
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            var ids = new List<string>();
            while (!process.StandardOutput.EndOfStream)
            {
                var line = process.StandardOutput.ReadLine();
                if (!Regex.IsMatch(line, androidDeviceRegex))
                    continue;

                ids.Add(Regex.Replace(line, androidDeviceRegex, string.Empty));
            }

            if(!string.IsNullOrEmpty(process.StandardError.ReadToEnd()))
                return Array.Empty<AndroidDevice>();

            // List of devices attached
            var devices = new List<AndroidDevice>();
            foreach (var deviceId in ids)
            {
                using var propProcess = new Process
                {
                    StartInfo = new ProcessStartInfo(ToolPath, $"-s {deviceId} shell getprop")
                    {
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    }
                };
                process.Start();
                var props = new List<string>();
                while (!propProcess.StandardOutput.EndOfStream)
                    props.Add(propProcess.StandardOutput.ReadLine());

                if (!string.IsNullOrEmpty(propProcess.StandardError.ReadToEnd()))
                    continue;

                devices.Add(new AndroidDevice(deviceId, props.ToArray()));
            }

            return devices;
        }
    }
}
