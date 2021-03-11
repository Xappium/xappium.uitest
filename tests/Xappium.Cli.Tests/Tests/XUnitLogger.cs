using System;
using System.IO;
using Xappium.Logging;

namespace Xappium.Cli.Tests
{
    public class XUnitLogger
    {
        public static void Init()
        {
            var testDir = new DirectoryInfo(Path.Combine(Environment.CurrentDirectory, "test"));
            if (testDir.Exists)
                testDir.Delete(true);

            testDir.Create();

            Logger.SetWorkingDirectory(testDir.FullName);
        }
    }
}
