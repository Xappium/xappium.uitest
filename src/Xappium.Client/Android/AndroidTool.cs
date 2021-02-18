using System;
using System.IO;
using System.Linq;

namespace Xappium.Client.Android
{
    internal static class AndroidTool
    {
        private static readonly string s_userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);

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

        //public static string GetJavaHome()
        //{
        //    var javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
        //    if (!string.IsNullOrEmpty(javaHome))
        //        return javaHome;

        //    var searchPaths = new[]
        //    {
        //        Path.Combine(s_userProfile, "Library", "Developer", "Xamarin", "jdk"),
        //    };

        //    foreach (var path in searchPaths)
        //    {
        //        if (Directory.Exists(path))
        //        {
        //            javaHome = path;
        //            var di = new DirectoryInfo(path);
        //            if (di.GetDirectories().Any(x => x.Name.Contains("microsoft_dist_openjdk")))
        //            {
        //                javaHome = di.GetDirectories().First(x => x.Name.Contains("microsoft_dist_openjdk")).FullName;
        //            }

        //            if (!string.IsNullOrEmpty(javaHome))
        //                return javaHome;
        //        }
        //    }

        //    return null;
        //}

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
            if (string.IsNullOrEmpty(path))
                throw new Exception($"No path was found for {name}. Be sure that the ANDROID_HOME or JAVA_HOME has been set and that the adb, sdkmanager, and emulator tools are installed");
        }
    }
}
