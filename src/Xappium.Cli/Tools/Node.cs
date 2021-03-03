using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;

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

        public static async Task<bool> InstallPackage(string packageName, CancellationToken cancellationToken)
        {
            Console.WriteLine($"npm install -g {packageName}");
            var stdErrBuffer = new StringBuilder();
            var stdOut = PipeTarget.ToDelegate(l => Console.WriteLine(l));
            await Cli.Wrap("npm")
                .WithArguments($"install -g {packageName}")
                .WithStandardOutputPipe(stdOut)
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return false;

            var errorOutput = stdErrBuffer.ToString().Trim();
            if (string.IsNullOrEmpty(errorOutput))
                return true;

            throw new Exception(errorOutput);
        }
    }
}
