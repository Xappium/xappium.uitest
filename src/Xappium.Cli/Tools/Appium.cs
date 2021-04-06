using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using Xappium.Logging;

namespace Xappium.Tools
{
    internal static class Appium
    {
        private const string defaultLog = "appium.log";

        public static string Address { get; set; } = "127.0.0.1";
        public static int Port { get; set; } = 4723;

        public static string Version
        {
            get
            {
                var toolPath = EnvironmentHelper.GetToolPath("appium");
                if (string.IsNullOrEmpty(toolPath))
                    return null;

                var address = string.Empty;
                if (!string.IsNullOrEmpty(Address))
                    address = $"--address {Address}";

                var port = $"--port {Port}";
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo(toolPath, $"{address} {port} -v")
                    {
                        CreateNoWindow = true,
                        RedirectStandardOutput = true
                    },
                };
                try
                {
                    process.Start();
                    while (!process.StandardOutput.EndOfStream)
                    {
                        var line = process.StandardOutput.ReadLine();
                        if (!string.IsNullOrEmpty(line))
                        {
                            Logger.WriteLine($"Appium: {line} installed", LogLevel.Normal);
                            return line;
                        }
                    }
                }
                catch(Win32Exception)
                {
                    Logger.WriteWarning("Appium is not currently installed");
                }

                return null;
            }
        }

        public static async Task<bool> Install(CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(Version))
                return true;

            return Node.IsInstalled && await Node.InstallPackage("appium", cancellationToken).ConfigureAwait(false);
        }

        public static Task<IDisposable> Run(string baseWorkingDirectory)
        {
            if (string.IsNullOrEmpty(Version))
                throw new Exception("Appium is not installed.");

            var completed = false;
            var tcs = new TaskCompletionSource<IDisposable>();
            var cancellationSource = new CancellationTokenSource();
            var logDirectory = Path.Combine(baseWorkingDirectory, "logs");
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            void HandleConsoleLine(string line)
            {
                if (line.Contains("listener started on ") && line.Contains($":{Port}"))
                {
                    Logger.WriteLine(line, LogLevel.Minimal, defaultLog);

                    if(!completed)
                        tcs.SetResult(new AppiumTask(cancellationSource));
                    completed = true;
                }
                else if(line.Contains("make sure there is no other instance of this server running already") ||
                    line.Contains("listen EADDRINUSE: address already in use"))
                {
                    Logger.WriteWarning(line, defaultLog);

                    if (!completed)
                        tcs.SetResult(new AppiumTask(cancellationSource));
                    completed = true;
                }
                else
                {
                    Logger.WriteLine(line, LogLevel.Verbose, defaultLog);
                }
            }

            var stdOut = PipeTarget.ToDelegate(HandleConsoleLine);
            var stdErr = PipeTarget.Merge(
                PipeTarget.ToFile(Path.Combine(logDirectory, "appium-error.log")),
                PipeTarget.ToDelegate(HandleConsoleLine));
            Logger.WriteLine("Starting Appium...", LogLevel.Minimal);

            var toolPath = EnvironmentHelper.GetToolPath("appium");
            var cmd = Cli.Wrap(toolPath)
                .WithStandardOutputPipe(stdOut)
                .WithStandardErrorPipe(stdErr)
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync(cancellationSource.Token);

            return tcs.Task;
        }

        private class AppiumTask : IDisposable
        {
            private CancellationTokenSource _tokenSource { get; }

            public AppiumTask(CancellationTokenSource tokenSource)
            {
                _tokenSource = tokenSource;
            }

            public void Dispose()
            {
                _tokenSource.Cancel();
                _tokenSource.Dispose();
            }
        }
    }
}
