using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using AndroidXml;

namespace Xappium.Android
{
    public static class ApkHelper
    {
        public const int DefaultSdkLevel = 29;

        public static int GetAndroidSdkVersion(int? specificVersion, string binDir)
        {
            if (specificVersion.HasValue)
                return specificVersion.Value;

            string tempDir = null;
            try
            {
                var apkFile = Directory.EnumerateFiles(binDir, "*.apk", SearchOption.AllDirectories).FirstOrDefault();
                tempDir = Path.Combine(Path.GetTempPath(), Path.GetFileNameWithoutExtension(apkFile));
                if (Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
                ZipFile.ExtractToDirectory(apkFile, tempDir);

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
                        if (reader.MoveToAttribute("targetSdkVersion", "http://schemas.android.com/apk/res/android"))
                            return int.Parse(reader.Value);

                        throw new Exception("android:targetSdkVersion attribute not found...");
                    }
                }
            }
            finally
            {
                if (!string.IsNullOrEmpty(tempDir) && Directory.Exists(tempDir))
                    Directory.Delete(tempDir, recursive: true);
            }

            return DefaultSdkLevel;
        }
    }
}
