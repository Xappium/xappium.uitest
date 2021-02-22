using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;

namespace Xappium.Tools
{
    internal static class Appium
    {
        public static bool Install()
        {
            return Node.IsInstalled && Node.InstallPackage("appium");
        }

        public static Task<IDisposable> Run(string baseWorkingDirectory)
        {
            var completed = false;
            var tcs = new TaskCompletionSource<IDisposable>();
            var cancellationSource = new CancellationTokenSource();
            var logDirectory = Path.Combine(baseWorkingDirectory, "logs");
            if (!Directory.Exists(logDirectory))
                Directory.CreateDirectory(logDirectory);

            void CompleteTask(string line)
            {
                if (completed)
                    return;

                if (line.Contains("listener started on 0.0.0.0:4723") ||
                    line.Contains("make sure there is no other instance of this server running already") ||
                    line.Contains("listen EADDRINUSE: address already in use 0.0.0.0:4723"))
                {
                    completed = true;
                    tcs.SetResult(cancellationSource);
                }
            }

            var stdOut = PipeTarget.Merge(
                PipeTarget.ToFile(Path.Combine(logDirectory, "appium.log")),
                PipeTarget.ToDelegate(l => Console.WriteLine(l)),
                PipeTarget.ToDelegate(CompleteTask));
            var stdErr = PipeTarget.Merge(
                PipeTarget.ToFile(Path.Combine(logDirectory, "appium-error.log")),
                PipeTarget.ToDelegate(CompleteTask));
            Console.WriteLine("Starting Appium...");
            var cmd = Cli.Wrap("appium")
                .WithStandardOutputPipe(stdOut)
                .WithStandardErrorPipe(stdErr)
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync(cancellationSource.Token);

            return tcs.Task;
        }
    }
}
