using System;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;

namespace Xappium.Client.Apple
{
    internal class AppleDeviceInfo
    {
        [JsonPropertyName("dataPath")]
        public string DataPath { get; set; }

        [JsonPropertyName("logPath")]
        public string LogPath { get; set; }

        [JsonPropertyName("udid")]
        public string Udid { get; set; }

        [JsonPropertyName("isAvailable")]
        public bool IsAvailable { get; set; }

        [JsonPropertyName("deviceTypeIdentifier")]
        public string DeviceTypeIdentifier { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        [JsonPropertyName("state")]
        public SimulatorState State { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonIgnore]
        public string OSVersion { get; set; }

        public void Boot()
        {
            Console.WriteLine($"Booting {Name} simulator.");
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo("xcrun", $"simctl boot {Udid}")
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            process.WaitForExit();

            var devices = AppleSimulator.GetAvailableSimulators();
            State = devices.First(x => x.Udid == Udid).State;

            if (State == SimulatorState.Booted)
                Console.WriteLine($"{Name} booted.");
            else
                Console.WriteLine($"{Name} was unable to complete booting.");
        }
    }
}
