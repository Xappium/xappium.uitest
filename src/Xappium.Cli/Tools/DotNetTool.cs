using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Builders;
using McMaster.Extensions.CommandLineUtils;
using Xappium.Logging;

namespace Xappium.Tools
{
    internal static class DotNetTool
    {
        public static Task Test(string projectPath, string outputPath, string configuration, string resultsDirectory, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(configuration))
                configuration = "Release";

            var baseDirectory = new DirectoryInfo(resultsDirectory).Parent.FullName;
            var logFile = Path.Combine(baseDirectory, "logs", "vstest.log");
            var args = new ArgumentsBuilder().Add("test")
                     .Add($"{projectPath}")
                     .Add($"--output={outputPath}")
                     .Add($"--configuration={configuration}")
                     .Add("--no-build")
                     .Add($"--results-directory={resultsDirectory}")
                     .Add($"--logger:trx;LogFileName={Path.GetFileNameWithoutExtension(projectPath)}.trx")
                     .Add($"--diag:{logFile}")
                     .Build();

            Logger.WriteLine($"Running dotnet test on '{projectPath}'", LogLevel.Minimal);
            return Execute(args, LogLevel.Normal, cancellationToken);
        }

        public static Task Build(Action<ArgumentsBuilder> configure, CancellationToken cancellationToken)
        {
            var builder = new ArgumentsBuilder()
                .Add("build");
            configure(builder);

            return Execute(builder.Build(), LogLevel.Detailed, cancellationToken);
        }

        private static async Task Execute(string args, LogLevel logLevel, CancellationToken cancellationToken)
        {
            Logger.WriteLine($"{DotNetExe.FullPath} {args}", LogLevel.Normal);
            var stdErrBuffer = new StringBuilder();
            var stdOut = PipeTarget.ToDelegate(l => Logger.WriteLine(l, logLevel));
            var stdErr = PipeTarget.ToStringBuilder(stdErrBuffer);

            var result = await Cli.Wrap(DotNetExe.FullPath)
                .WithArguments(args)
                .WithStandardErrorPipe(stdOut)
                .WithStandardErrorPipe(stdErr)
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync(cancellationToken)
                .ConfigureAwait(false);

            var error = stdErrBuffer.ToString().Trim();
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            if (result.ExitCode > 0)
                throw new Exception($"The dotnet tool unexpectidly exited without any error output with code: {result.ExitCode}");
        }
    }
}
