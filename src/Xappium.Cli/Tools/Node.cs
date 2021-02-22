using System;
using System.Linq;

namespace Xappium.Tools
{
    internal static class Node
    {
        public static string Version
        {
            get
            {
                var result = ProcessHelper.Run("node", "-v", displayRealtimeOutput: true);
                if (result.IsErred)
                    return null;

                return result.Output.FirstOrDefault(x => x.StartsWith("v"));
            }
        }

        public static bool IsInstalled => !string.IsNullOrEmpty(Version);

        public static bool InstallPackage(string packageName)
        {
            var result = ProcessHelper.Run("npm", $"install -g {packageName}", displayRealtimeOutput: true);
            return !result.IsErred;
        }
    }
}
