using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Appium.TestHelpers.Tools
{
    public class AppleDeviceList
    {
        [JsonPropertyName("devices")]
        public Dictionary<string, IEnumerable<AppleDeviceInfo>> Devices { get; set; }
    }
}
