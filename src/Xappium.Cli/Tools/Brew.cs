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

        public static async Task Tap(string source, CancellationToken cancellationToken)
        {
            await ExecuteInternal(x => x.Add("update"), cancellationToken);
            await ExecuteInternal(x => x.Add("tap").Add(source), cancellationToken);
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

        internal static async Task ExecuteInternal(Action<ArgumentsBuilder> configure, CancellationToken cancellationToken)
        {
            if (cancellationToken.IsCancellationRequested)
                return;

            var builder = new ArgumentsBuilder();
            configure(builder);
            var args = builder.Build();
            Logger.WriteLine($"{ToolPath} {args}", LogLevel.Normal);
            var stdOutBuffer = new StringBuilder();
            var stdOut = PipeTarget.Merge(PipeTarget.ToStringBuilder(stdOutBuffer),
                PipeTarget.ToDelegate(l => Logger.WriteLine(l, LogLevel.Verbose)));
            var stdError = PipeTarget.ToDelegate(l =>
            {
                if (string.IsNullOrEmpty(l))
                    return;

                // Suppress errors
                Logger.WriteWarning(l);
            });

            var result = await Cli.Wrap(ToolPath)
                .WithArguments(args)
                .WithValidation(CommandResultValidation.None)
                .WithStandardErrorPipe(stdError)
                .WithStandardOutputPipe(stdOut)
                .ExecuteAsync(cancellationToken);
        }
    }
}
