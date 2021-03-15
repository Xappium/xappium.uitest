using System;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace Xappium.Android
{
    internal static class AndroidTool
    {
        private static readonly string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
        private static readonly string programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);
        private static readonly string programFilesX86 = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

        static AndroidTool()
        {
            if(EnvironmentHelper.IsRunningOnMac)
            {
                s_java_home = GetMacOSJavaHome();
            }
            else
            {
                s_java_home = new[]
                {
                    Path.Combine(programFiles, "Android"),
                    Path.Combine(programFilesX86, "Android"),
                }
                .Where(x => Directory.Exists(x))
                .SelectMany(x => Directory.EnumerateFiles(x, "java.exe", SearchOption.AllDirectories))
                .Select(x => new FileInfo(x).Directory.FullName)
                .FirstOrDefault();
            }
        }

        private static readonly string s_java_home;

        private static string[] s_androidSdkPaths =>
            new[]
            {
                Environment.GetEnvironmentVariable("ANDROID_HOME"),
                Environment.GetEnvironmentVariable("ANDROID_SDK_ROOT"),
                Path.Combine(userProfile, "AppData", "Local", "Android", "Sdk"),
                Path.Combine(userProfile, "Library", "Android", "sdk"),
                Path.Combine(userProfile, "android-toolchain", "sdk"),
                Path.Combine(userProfile, "Library", "Developer", "Xamarin", "android-sdk-macosx"),
                Path.Combine(programFiles, "Android", "android-sdk"),
                Path.Combine(programFilesX86, "Android", "android-sdk"),
            }
            .Where(x => !string.IsNullOrEmpty(x) && Directory.Exists(x))
            .ToArray();

        private static string[] s_searchPaths =>
            s_androidSdkPaths.SelectMany(x => new[]
            {
                x,
                Path.Combine(x, "cmdline-tools", "latest", "bin"),
                Path.Combine(x, "cmdline-tools", "2.0", "bin"),
                Path.Combine(x, "cmdline-tools", "1.0", "bin"),
                Path.Combine(x, "emulator"),
                Path.Combine(x, "tools"),
                Path.Combine(x, "tools", "bin"),
                Path.Combine(x, "platform-tools")
            })
            .Where(x => Directory.Exists(x))
            .ToArray();

        private static string GetMacOSJavaHome()
        {
            var process = new Process
            {
                StartInfo = new ProcessStartInfo("echo", "$(/usr/libexec/java_home)")
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            process.Start();
            while (!process.StandardOutput.EndOfStream)
            {
                return process.StandardOutput.ReadLine();
            }

            return null;
        }

        public static void ValidateEnvironmentSettings()
        {
            var androidHome = Environment.GetEnvironmentVariable("ANDROID_HOME");
            if (string.IsNullOrEmpty(androidHome))
            {
                androidHome = s_androidSdkPaths.FirstOrDefault(x => Directory.Exists(x)) ?? throw new DirectoryNotFoundException("Could not locate the Android Home directory");
                Environment.SetEnvironmentVariable("ANDROID_HOME", androidHome, EnvironmentVariableTarget.Machine);
            }

            var javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (string.IsNullOrWhiteSpace(javaHome))
            {
                if(string.IsNullOrEmpty(s_java_home))
                {
                    throw new DirectoryNotFoundException("Could not locate the Java Home directory");
                }

                Environment.SetEnvironmentVariable("JAVA_HOME", s_java_home, EnvironmentVariableTarget.Machine);
            }
        }

        internal static string LocateUtility(string fileName)
        {
            var path = EnvironmentHelper.GetToolPath(fileName);
            if (!string.IsNullOrEmpty(path))
                return path;

            foreach (var dir in s_searchPaths)
            {
                var file = LocateInternal(dir, fileName);

                if (!string.IsNullOrEmpty(file))
                {
                    return file;
                }
            }

            return null;
        }

        private static string LocateInternal(string dir, string fileName)
        {
            return new[]
                {
                    new FileInfo(Path.Combine(dir, fileName)),
                    new FileInfo(Path.Combine(dir, $"{fileName}.bat")),
                    new FileInfo(Path.Combine(dir, $"{fileName}.exe")),
                }
                .Where(x => x.Exists)
                .Select(x => x.FullName)
                .FirstOrDefault();
        }

        internal static void ThrowIfNull(string path, string name)
        {
            ValidateEnvironmentSettings();

            if (string.IsNullOrEmpty(path))
                throw new Exception($"No path was found for {name}. Be sure that the ANDROID_HOME or JAVA_HOME has been set and that the adb, sdkmanager, and emulator tools are installed");
        }
    }
}
