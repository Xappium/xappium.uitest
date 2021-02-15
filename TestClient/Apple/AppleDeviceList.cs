using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace TestClient.Apple
{
    public class AppleDeviceList
    {
        [JsonPropertyName("devices")]
        public Dictionary<string, IEnumerable<AppleDeviceInfo>> Devices { get; set; }
    }
}
