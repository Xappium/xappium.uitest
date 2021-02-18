using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Xappium.UITest.Extensions;
using Newtonsoft.Json;

namespace Xappium.UITest.Configuration
{
    public class UITestConfiguration
    {
        public Platform Platform { get; set; }

        public string AppPath { get; set; }

        public string AppId { get; set; }

        public string DeviceName { get; set; }

        public string UDID { get; set; }

        public string OSVersion { get; set; }

        public string ScreenshotsPath { get; set; }

        public Uri AppiumServer { get; set; }

        public Dictionary<string, string> Capabilities { get; set; } = new Dictionary<string, string>();

        public Dictionary<string, string> Settings { get; set; } = new Dictionary<string, string>();

        internal void Merge(UITestConfiguration config)
        {
            if (config.Platform != Platform.NotSet)
                Platform = config.Platform;

            if (!string.IsNullOrEmpty(config.AppPath))
                AppPath = config.AppPath;

            if (!string.IsNullOrEmpty(config.AppId))
                AppId = config.AppId;

            if (!string.IsNullOrEmpty(config.DeviceName))
                DeviceName = config.DeviceName;

            if (!string.IsNullOrEmpty(config.UDID))
                UDID = config.UDID;

            if (!string.IsNullOrEmpty(config.OSVersion))
                OSVersion = config.OSVersion;

            if (!string.IsNullOrEmpty(config.ScreenshotsPath))
                ScreenshotsPath = config.ScreenshotsPath;

            if (config.AppiumServer != null)
                AppiumServer = config.AppiumServer;

            if(config.Settings.Any())
            {
                foreach((var key, var value) in config.Settings)
                {
                    Settings[key] = value;
                }
            }

            if(config.Capabilities.Any())
            {
                foreach((var key, var value) in config.Capabilities)
                {
                    Capabilities[key] = value;
                }
            }
        }

        internal void Merge(string json) =>
            Merge(JsonConvert.DeserializeObject<UITestConfiguration>(json));

        internal void Merge(Stream stream)
        {
            using var reader = new StreamReader(stream);
            Merge(reader.ReadToEnd());
        }
    }
}
