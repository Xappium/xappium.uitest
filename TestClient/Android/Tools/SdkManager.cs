using System;
using System.Diagnostics;
using static TestClient.Android.AndroidTool;

namespace TestClient.Android
{
    public static class SdkManager
    {
        public static readonly string ToolPath = LocateUtility("sdkmanager");

        public static void EnsureSdkIsInstalled(int version = 29)
        {
            ThrowIfNull(ToolPath, nameof(SdkManager));

            var installArgs = $"system-images;android-{version};google_apis;x86";
            using var process = new Process
            {
                StartInfo = new ProcessStartInfo(ToolPath, $"--install '{installArgs}'")
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            // Echo y
            while (!process.StandardOutput.EndOfStream)
                Console.WriteLine(process.StandardOutput.ReadLine());
        }
    }
}
