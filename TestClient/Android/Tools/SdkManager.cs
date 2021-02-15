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
            var result = ProcessHelper.Run(ToolPath, $"--install '{installArgs}'");
            // echo y
            if (result.IsErred)
                throw new Exception(result.Error);
        }
    }
}
