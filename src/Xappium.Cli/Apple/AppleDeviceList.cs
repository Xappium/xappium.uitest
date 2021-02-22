using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Xappium.Apple
{
    internal class AppleDeviceList
    {
        [JsonPropertyName("devices")]
        public Dictionary<string, IEnumerable<AppleDeviceInfo>> Devices { get; set; }
    }
}
