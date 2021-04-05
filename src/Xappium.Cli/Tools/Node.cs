using System;
using System.Collections.Generic;
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
            var toolPath = EnvironmentHelper.GetToolPath("npm");
            Logger.WriteLine($"{toolPath} install -g {packageName}", LogLevel.Verbose);
            var isMac = EnvironmentHelper.IsRunningOnMac;
            var errorLines = new List<string>();
            var stdOut = PipeTarget.ToDelegate(l => Logger.WriteLine(l, LogLevel.Verbose));
            var stdErr = PipeTarget.ToDelegate(l =>
            {
                if (string.IsNullOrEmpty(l) || (isMac && l.Contains("did not detect a Windows system")))
                    return;
                errorLines.Add(l);
            });
            await Cli.Wrap(toolPath)
                //.WithArguments($"install -g {packageName}")
                .WithArguments(b => b.Add("install")
                                .Add("-g")
                                .Add(packageName))
                .WithValidation(CommandResultValidation.None)
                .WithStandardOutputPipe(stdOut)
                .WithStandardErrorPipe(stdErr)
                .ExecuteAsync(cancellationToken);

            if (cancellationToken.IsCancellationRequested)
                return false;

            if (errorLines.Any())
                return true;

            throw new Exception(string.Join(Environment.NewLine, errorLines));
        }
    }
}
