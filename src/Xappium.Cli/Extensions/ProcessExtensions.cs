using System.Diagnostics;

namespace Xappium.Extensions
{
    internal static class ProcessExtensions
    {
        public static bool IsErred(this Process process)
        {
            var output = process.StandardError.ReadToEnd().Trim();
            return !string.IsNullOrEmpty(output);
        }

        public static string ErrorMessage(this Process process)
        {
            return process.StandardError.ReadToEnd().Trim();
        }
    }
}
