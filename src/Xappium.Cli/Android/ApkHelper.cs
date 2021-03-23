using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Xml;
using AndroidXml;

namespace Xappium.Android
{
    public static class ApkHelper
    {
        public const int DefaultSdkLevel = 29;
        private const string AndroidNamespace = "http://schemas.android.com/apk/res/android";

        public static int GetAndroidSdkVersion(int? specificVersion, string binDir)
        {
            if (specificVersion.HasValue)
                return specificVersion.Value;

            string tempDir = null;
            try
            {
                tempDir = ExpandApk(binDir);
                var manifest = Directory.EnumerateFiles(tempDir, "AndroidManifest.xml", SearchOption.AllDirectories).FirstOrDefault();
                if (string.IsNullOrEmpty(manifest))
                    return DefaultSdkLevel;

                using var stream = File.OpenRead(manifest);
                using var reader = new AndroidXmlReader(stream);
                while (reader.Read())
                {
                    // <uses-sdk android:minSdkVersion="21" android:targetSdkVersion="29" />
                    if(reader.NodeType == System.Xml.XmlNodeType.Element && reader.Name == "uses-sdk")
                    {
                        if (reader.MoveToAttribute("targetSdkVersion", AndroidNamespace))
                            return int.Parse(reader.Value);

                        throw new Exception("android:targetSdkVersion attribute not found...");
                    }
                }
            }
            finally
            {
                CleanupTempDir(tempDir);
            }

            return DefaultSdkLevel;
        }

        public static string GetAppActivity(string binDir)
        {
            string tempDir = null;
            try
            {
                tempDir = ExpandApk(binDir);
                var manifest = Directory.EnumerateFiles(tempDir, "AndroidManifest.xml", SearchOption.AllDirectories).FirstOrDefault();
                using var stream = File.OpenRead(manifest);
                using var reader = new AndroidXmlReader(stream);
                var activities = new List<string>();
                while (reader.Read())
                {
                    /* <application android:label="TestApp.Android" android:theme="@style/MainTheme" android:name="android.app.Application" android:allowBackup="true" android:icon="@mipmap/icon" android:debuggable="true" android:extractNativeLibs="true">
                        <activity android:configChanges="orientation|smallestScreenSize|screenLayout|screenSize|uiMode" android:icon="@mipmap/icon" android:label="TestApp" android:theme="@style/MainTheme" android:name="crc64c7fff72bd74cd533.MainActivity">
                          <intent-filter>
                            <action android:name="android.intent.action.MAIN" />
                            <category android:name="android.intent.category.LAUNCHER" />
                          </intent-filter>
                        </activity> */

                    if(reader.NodeType == XmlNodeType.Element
                        && reader.Name == "activity"
                        && reader.MoveToAttribute("name", AndroidNamespace))
                    {
                        activities.Add(reader.Value);
                    }
                }

                // TODO: We may want to look at the intents
                return activities.FirstOrDefault(x => x.EndsWith(".MainActivity"));
            }
            finally
            {
                CleanupTempDir(tempDir);
            }
        }

        private static string ExpandApk(string binDir)
        {
            var apkFile = Directory.EnumerateFiles(binDir, "*.apk", SearchOption.AllDirectories).FirstOrDefault();
            var tempDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(apkFile));
            if (Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
            ZipFile.ExtractToDirectory(apkFile, tempDir);
            return tempDir;
        }

        private static void CleanupTempDir(string tempDir)
        {
            if (!string.IsNullOrEmpty(tempDir) && Directory.Exists(tempDir))
                Directory.Delete(tempDir, recursive: true);
        }
    }
}
