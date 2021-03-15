using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Builders;
using Xappium.Logging;

namespace Xappium.Tools
{
    public static class Brew
    {
        public static readonly string ToolPath = EnvironmentHelper.GetToolPath("brew");

        public static Task Install(string packageName, CancellationToken cancellationToken)
        {
            return ExecuteInternal(x => x.Add("install").Add(packageName), cancellationToken);
        }

        public static Task Tap(string source, CancellationToken cancellationToken)
        {
            return ExecuteInternal(x => x.Add("tap").Add(source), cancellationToken);
        }

        public static async Task InstallIdbCompanion(CancellationToken cancellationToken)
        {
            await Tap("facebook/fb", cancellationToken);
            await Install("idb-companion", cancellationToken);
        }

        public static async Task InstallAppleSimUtils(CancellationToken cancellationToken)
        {
            await Tap("wix/brew", cancellationToken);
            await Install("applesimutils", cancellationToken);
        }

        public static Task InstallFFMPEG(CancellationToken cancellation) =>
            Install("ffmpeg", cancellation);

        internal static async Task<string> ExecuteInternal(Action<ArgumentsBuilder> configure, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return null;

            var toolPath = ToolPath;
            var builder = new ArgumentsBuilder();
            configure(builder);
            var args = builder.Build();
            Logger.WriteLine($"{toolPath} {args}", LogLevel.Normal);
            var stdErrBuffer = new StringBuilder();
            var stdOutBuffer = new StringBuilder();
            var stdOut = PipeTarget.Merge(PipeTarget.ToStringBuilder(stdOutBuffer),
                PipeTarget.ToDelegate(l => Logger.WriteLine(l, LogLevel.Verbose)));

            var result = await Cli.Wrap(toolPath)
                .WithArguments(args)
                .WithValidation(CommandResultValidation.None)
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .WithStandardOutputPipe(stdOut)
                .ExecuteAsync(cancellationToken);

            var stdErr = stdErrBuffer.ToString().Trim();
            if (!string.IsNullOrEmpty(stdErr))
                throw new Exception(stdErr);

            if (result.ExitCode != 0)
                throw new Exception("Brew exited with non-zero exit code.");

            return stdOutBuffer.ToString().Trim();
        }
    }
}
