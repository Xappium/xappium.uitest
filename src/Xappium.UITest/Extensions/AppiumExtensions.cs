using System;
using System.IO;
using OpenQA.Selenium.Appium;

namespace Xappium.UITest.Extensions
{
    internal static class AppiumExtensions
    {
        // Espresso build config is supposed to be ok with a json string
        // but the dotnet driver doesn't seem to work with this, and it must instead be
        // a file path to a json file.
        // This will check if the config option is a json string and create a temp file
        // to pass instead if so
        public static void FixEspressoBuildConfigOptions(this AppiumOptions options)
        {
            const string espressoBuildConfigCapability = "espressoBuildConfig";

            var od = options.ToDictionary();

            if (od.TryGetValue(espressoBuildConfigCapability, out var bc))
            {
                var bcStr = bc?.ToString() ?? string.Empty;

                if (!string.IsNullOrEmpty(bcStr))
                {
                    // Check if it's a valid file and if so we don't need to do anything
                    if (bcStr.IndexOfAny(Path.GetInvalidFileNameChars()) < 0 || !File.Exists(bcStr))
                    {
                        try
                        {
                            var bcJson = Newtonsoft.Json.Linq.JObject.Parse(bc.ToString());
                            if (bcJson != null)
                            {
                                var tmpFile = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString() + "-espressoBuildConfig.json");
                                File.WriteAllText(tmpFile, bcJson.ToString(Newtonsoft.Json.Formatting.Indented));

                                // Overwrite original capability with temp file
                                options.AddAdditionalCapability(espressoBuildConfigCapability, tmpFile);
                            }
                        }
                        catch { }
                    }
                }
            }
        }
    }
}
