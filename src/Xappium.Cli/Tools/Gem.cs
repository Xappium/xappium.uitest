using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Builders;
using Xappium.Logging;

namespace Xappium.Tools
{
    public static class Gem
    {
        public static readonly string ToolPath = EnvironmentHelper.GetToolPath("gem");

        public static Task Install(string packageName, CancellationToken cancellationToken)
        {
            return ExecuteInternal(b =>
            {
                b.Add("install")
                 .Add(packageName);
            }, cancellationToken);
        }

        public static Task InstallXcPretty(CancellationToken cancellationToken) =>
            Install("xcpretty", cancellationToken);

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
            {
                if (stdErr.Split('\n').Select(x => x.Trim()).All(x => x.StartsWith("Warning:", StringComparison.InvariantCultureIgnoreCase)))
                    Logger.WriteWarning(stdErr);
                else
                    throw new Exception(stdErr);
            }

            return stdOutBuffer.ToString().Trim();
        }
    }
}
