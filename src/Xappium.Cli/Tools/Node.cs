using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Xappium.Logging;

namespace Xappium.Tools
{
    internal static class Node
    {
        public static string Version
        {
            get
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo("node", "-v")
                    {
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    },
                };
                process.Start();
                while(!process.StandardOutput.EndOfStream)
                {
                    var line = process.StandardOutput.ReadLine();
                    if (line.StartsWith("v"))
                    {
                        Logger.WriteLine($"Node: {line} installed", LogLevel.Normal);
                        return line;
                    }
                }

                return null;
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
                .WithValidation(CommandResultValidation.None)
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
