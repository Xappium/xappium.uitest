using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Xappium.Client.Apple
{
    internal class AppleDeviceList
    {
        [JsonPropertyName("devices")]
        public Dictionary<string, IEnumerable<AppleDeviceInfo>> Devices { get; set; }
    }
}
