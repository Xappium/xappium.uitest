using System;
using System.Diagnostics;
using static Xappium.Android.AndroidTool;

namespace Xappium.Android
{
    internal static class SdkManager
    {
        public static readonly string ToolPath = LocateUtility("sdkmanager");

        public static void InstallWebDriver()
        {
            ThrowIfNull(ToolPath, nameof(SdkManager));

            var result = ProcessHelper.Run(ToolPath, @"--install ""extras;google;webdriver""", displayRealtimeOutput: true, new[] { "y" });

            if (result.IsErred)
                throw new Exception(result.Error);
        }

        public static void EnsureSdkIsInstalled(int sdkVersion = 29)
        {
            ThrowIfNull(ToolPath, nameof(SdkManager));

            var installArgs = $"system-images;android-{sdkVersion};google_apis_playstore;x86";
            var result = ProcessHelper.Run(ToolPath, $"--install \"{installArgs}\"", displayRealtimeOutput: true, new[] { "y" });
            // echo y
            if (result.IsErred)
                throw new Exception(result.Error);
        }
    }
}
