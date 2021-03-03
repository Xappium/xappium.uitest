using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using Xappium.Android;
using Xappium.Apple;

namespace Xappium
{
    public static class EnvironmentHelper
    {
        public static readonly bool IsRunningOnMac = IsRunningOnMacInternal();

        public static readonly bool IsAndroidSupported = IsAndroidSupportedInternal();

        public static readonly bool IsIOSSupported = IsIOSSupportedInternal();

        private static readonly string[] s_systemPaths = Environment.GetEnvironmentVariable("PATH").Split(Path.PathSeparator);

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
                return IsRunningOnMac && AppleSimulator.GetAvailableSimulators().Any();
            }
            catch { }

            return false;
        }

        public static string GetToolPath(string toolName)
        {
            return s_systemPaths.SelectMany(x => new[]
                {
                    Path.Combine(x, toolName),
                    Path.Combine(x, $"{toolName}.exe"),
                    Path.Combine(x, $"{toolName}.bat")
                })
                .Where(x => File.Exists(x))
                .FirstOrDefault();
        }
    }
}
