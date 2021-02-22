using System.Collections.Generic;

namespace Xappium
{
    internal class TestConfiguration
    {
        public string Platform { get; set; }

        public string AppPath { get; set; }

        public string AppId { get; set; }

        public string DeviceName { get; set; }

        public string UDID { get; set; }

        public string OSVersion { get; set; }

        public string ScreenshotsPath { get; set; }

        public Dictionary<string, string> Capabilities { get; set; }

        public Dictionary<string, string> Settings { get; set; }
    }
}
