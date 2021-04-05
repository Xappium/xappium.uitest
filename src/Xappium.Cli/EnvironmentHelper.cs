using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xappium.Android;
using Xappium.Apple;
using Xappium.Logging;

namespace Xappium
{
    public static class EnvironmentHelper
    {
        private static readonly Lazy<bool> _lazyIsRunningOnMac = new Lazy<bool>(IsRunningOnMacInternal);
        private static readonly Lazy<bool> _lazyIsAndroidSupported = new Lazy<bool>(IsAndroidSupportedInternal);
        private static readonly Lazy<bool> _lazyIsIOSSupported = new Lazy<bool>(IsIOSSupportedInternal);

        public static bool IsRunningOnMac => _lazyIsRunningOnMac.Value;

        public static bool IsAndroidSupported => _lazyIsAndroidSupported.Value;

        public static bool IsIOSSupported => _lazyIsIOSSupported.Value;

        //From Managed.Windows.Forms/XplatUI
        [DllImport("libc")]
        private static extern int uname(IntPtr buf);
        private static bool IsRunningOnMacInternal()
        {
            IntPtr buf = IntPtr.Zero;
            try
            {
                buf = Marshal.AllocHGlobal(8192);
                // This is a hacktastic way of getting sysname from uname ()
                if (uname(buf) == 0)
                {
                    string os = Marshal.PtrToStringAnsi(buf);
                    if (os == "Darwin")
                        return true;
                }
            }
            catch { }
            finally
            {
                if (buf != IntPtr.Zero)
                    Marshal.FreeHGlobal(buf);
            }
            return false;
        }

        private static bool IsAndroidSupportedInternal()
        {
            try
            {
                AndroidTool.ValidateEnvironmentSettings();

                return !string.IsNullOrEmpty(SdkManager.ToolPath);
            }
            catch
            {
                return false;
            }
        }

        private static bool IsIOSSupportedInternal()
        {
            try
            {
                var hasSimulators = AppleSimulator.GetAvailableSimulators().Any();
                if(IsRunningOnMac && !hasSimulators)
                    Logger.WriteError("You appear to be running on macOS, but there are no available Simulators installed.");

                return IsRunningOnMac && hasSimulators;
            }
            catch { }

            return false;
        }

        public static string GetToolPath(string toolName)
        {
            // For some reason this is null if we create a static readonly property.
            var systemPaths = Environment.GetEnvironmentVariable("PATH")
                                   .Split(Path.PathSeparator)
                                   .Where(x => !string.IsNullOrEmpty(x) && Directory.Exists(x));
            return systemPaths.SelectMany(x => new[]
                {
                    Path.Combine(x, $"{toolName}.exe"),
                    Path.Combine(x, $"{toolName}.bat"),
                    Path.Combine(x, $"{toolName}.cmd"),
                    Path.Combine(x, toolName),
                })
                .Where(x => File.Exists(x))
                .FirstOrDefault();
        }
    }
}
