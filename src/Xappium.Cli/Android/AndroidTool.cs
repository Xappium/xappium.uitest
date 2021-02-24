using System;
using System.IO;
using System.Linq;

namespace Xappium.Android
{
    internal static class AndroidTool
    {
        private static readonly string s_userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

        private static readonly string[] s_androidSdkPaths = new[]
        {
            Path.Combine(s_userProfile, "AppData", "Local", "Android", "Sdk"),
            Path.Combine(s_userProfile, "Library", "Android", "sdk"),
            Path.Combine(s_userProfile, "android-toolchain", "sdk"),
            Path.Combine(s_userProfile, "Library", "Developer", "Xamarin", "android-sdk-macosx"),
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Android", "android-sdk"),
        };

        private static readonly string[] s_searchPaths = new[]
            {
                Path.Combine(s_userProfile, "AppData", "Local", "Android", "Sdk"),
                Path.Combine(s_userProfile, "AppData", "Local", "Android", "Sdk", "cmdline-tools", "1.0", "bin"),
                Path.Combine(s_userProfile, "AppData", "Local", "Android", "Sdk", "cmdline-tools", "latest", "bin"),
                Path.Combine(s_userProfile, "AppData", "Local", "Android", "Sdk", "emulator"),
                Path.Combine(s_userProfile, "AppData", "Local", "Android", "Sdk", "tools"),
                Path.Combine(s_userProfile, "AppData", "Local", "Android", "Sdk", "tools", "bin"),
                Path.Combine(s_userProfile, "Library", "Android", "sdk", "platform-tools"),
                Path.Combine(s_userProfile, "Library", "Android", "sdk", "cmdline-tools", "1.0", "bin"),
                Path.Combine(s_userProfile, "Library", "Android", "sdk", "cmdline-tools", "latest", "bin"),
                Path.Combine(s_userProfile, "Library", "Android", "sdk", "emulator"),
                Path.Combine(s_userProfile, "Library", "Android", "sdk", "tools"),
                Path.Combine(s_userProfile, "Library", "Android", "sdk", "tools", "bin"),
                Path.Combine(s_userProfile, "android-toolchain", "sdk", "platform-tools"),
                Path.Combine(s_userProfile, "android-toolchain", "sdk", "cmdline-tools", "1.0", "bin"),
                Path.Combine(s_userProfile, "android-toolchain", "sdk", "cmdline-tools", "latest", "bin"),
                Path.Combine(s_userProfile, "android-toolchain", "sdk", "emulator"),
                Path.Combine(s_userProfile, "android-toolchain", "sdk", "tools"),
                Path.Combine(s_userProfile, "android-toolchain", "sdk", "tools", "bin"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Android", "android-sdk"),
                Path.Combine(s_userProfile, "Library", "Android", "sdk"),
                Path.Combine(s_userProfile, "Library", "Developer", "Xamarin", "android-sdk-macosx", "cmdline-tools", "latest", "bin"),
                Path.Combine(s_userProfile, "Library", "Developer", "Xamarin", "android-sdk-macosx", "cmdline-tools", "current", "bin"),
                Path.Combine(s_userProfile, "Library", "Developer", "Xamarin", "android-sdk-macosx", "cmdline-tools", "1.0", "bin"),
                Path.Combine(s_userProfile, "Library", "Developer", "Xamarin", "android-sdk-macosx"),
                Path.Combine(s_userProfile, "AppData", "Local", "Android", "Sdk", "platform-tools"),
            };

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
                // TODO: This needs to be updated to support running on Windows.
                var result = ProcessHelper.Run("echo", "$(/usr/libexec/java_home)");
                javaHome = result.Output.FirstOrDefault() ?? throw new DirectoryNotFoundException("Could not locate the Java Home");
                Environment.SetEnvironmentVariable("JAVA_HOME", javaHome, EnvironmentVariableTarget.Machine);
            }
        }

        internal static string LocateUtility(string fileName)
        {
            var path = new FileInfo(fileName);
            if (path.Exists)
                return path.FullName;

            foreach (var dir in s_searchPaths)
            {
                if (string.IsNullOrEmpty(dir) || !Directory.Exists(dir))
                {
                    continue;
                }

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
            var files = new[]
            {
                Path.Combine(dir, fileName),
                Path.Combine(dir, "platform-tools", fileName),
                Path.Combine(dir, "tools", fileName),
                Path.Combine(dir, "tools", "bin", fileName),
            };

            foreach (var file in files)
            {
                if (File.Exists(file))
                {
                    return file;
                }
            }

            return null;
        }

        internal static void ThrowIfNull(string path, string name)
        {
            ValidateEnvironmentSettings();

            if (string.IsNullOrEmpty(path))
                throw new Exception($"No path was found for {name}. Be sure that the ANDROID_HOME or JAVA_HOME has been set and that the adb, sdkmanager, and emulator tools are installed");
        }
    }
}
