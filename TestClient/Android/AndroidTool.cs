using System;
using System.IO;
using System.Linq;

namespace TestClient.Android
{
    internal static class AndroidTool
    {
        public static string GetAndroidHome()
        {
            var androidHome = Environment.GetEnvironmentVariable("ANDROID_HOME");
            if (!string.IsNullOrEmpty(androidHome))
                return androidHome;

            var searchPath = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86), "Android", "android-sdk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Android", "sdk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "AppData", "Local", "Android", "Sdk"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Developer", "Xamarin", "android-sdk-macosx"),
            };

            foreach (var path in searchPath)
            {
                if (Directory.Exists(path))
                {
                    // Environment.SetEnvironmentVariable("ANDROID_HOME", path);
                    return path;
                }
            }

            return null;
        }

        public static string GetJavaHome()
        {
            var javaHome = Environment.GetEnvironmentVariable("JAVA_HOME");
            if (!string.IsNullOrEmpty(javaHome))
                return javaHome;

            var searchPaths = new[]
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Library", "Developer", "Xamarin", "jdk"),
            };

            foreach (var path in searchPaths)
            {
                if (Directory.Exists(path))
                {
                    javaHome = path;
                    var di = new DirectoryInfo(path);
                    if (di.GetDirectories().Any(x => x.Name.Contains("microsoft_dist_openjdk")))
                    {
                        javaHome = di.GetDirectories().First(x => x.Name.Contains("microsoft_dist_openjdk")).FullName;
                    }

                    if (!string.IsNullOrEmpty(javaHome))
                        return javaHome;
                }
            }

            return null;
        }

        internal static string LocateUtility(string fileName)
        {
            var path = new FileInfo(fileName);
            if (path.Exists)
                return path.FullName;

            var searchPaths = new[]
            {
                GetJavaHome(),
                GetAndroidHome(),
            };

            foreach (var dir in searchPaths)
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
