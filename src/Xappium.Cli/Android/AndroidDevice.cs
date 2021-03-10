using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;

namespace Xappium.Android
{
    internal class AndroidDevice
    {
        private const string devicePropertyPattern = @"(?<=\[).+?(?=\])";

        internal AndroidDevice(string deviceId, string[] deviceProperties)
        {
            Id = deviceId;
            var properties = new Dictionary<string, string>();
            foreach (var prop in deviceProperties)
            {
                var matches = Regex.Matches(prop, devicePropertyPattern);
                string key, value = null;
                if (matches.Count > 0)
                {
                    key = matches[0].Value;
                }
                else
                {
                    continue;
                }

                if (matches.Count > 1)
                {
                    value = matches[1].Value;
                }

                properties.Add(key, value);
            }

            Properties = properties;
            Model = GetDeviceProp("ro.product.model");
            Name = GetDeviceProp("ro.product.name");
            Locale = GetDeviceProp("persist.sys.locale") ?? GetDeviceProp("ro.product.locale");
            Manufacturer = GetDeviceProp("ro.product.manufacturer") ?? GetDeviceProp("ro.product.vendor.manufacturer");
            SdkVersion = int.TryParse(GetDeviceProp("ro.build.version.sdk"), out var sdk) ? sdk : -1;

            // TODO: Determine why this may come back null and find alternate property...
            SupportedAbi = GetDeviceProp("ro.product.cpu.abilist")?.Split(',') ?? Array.Empty<string>();
        }

        public string Id { get; }

        public string Model { get; }

        public string Name { get; }

        public string Locale { get; }

        public string Manufacturer { get; }

        public int SdkVersion { get; }

        public IEnumerable<string> SupportedAbi { get; }

        public Dictionary<string, string> Properties { get; }

        public string Platform => "Android";

        public bool IsEmulator => CultureInfo.CurrentCulture.CompareInfo.IndexOf(Id, "emulator", CompareOptions.IgnoreCase) >= 0 || CultureInfo.CurrentCulture.CompareInfo.IndexOf(Name, "emulator", CompareOptions.IgnoreCase) >= 0;

        private string GetDeviceProp(string property)
        {
            if (Properties.ContainsKey(property))
                return Properties[property];
            return null;
        }
    }
}
