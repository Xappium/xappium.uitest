using System;
using System.Diagnostics;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Builders;
using Xappium.Logging;
using static Xappium.Android.AndroidTool;

namespace Xappium.Android
{
    internal static class SdkManager
    {
        public static readonly string ToolPath = LocateUtility("sdkmanager");

        public static Task InstallWebDriver(CancellationToken cancellationToken)
        {
            return ExecuteInternal(o => o.Add(@"--install ""extras;google;webdriver"""), cancellationToken);
        }

        public static Task EnsureSdkIsInstalled(int sdkVersion, CancellationToken cancellationToken)
        {
            var installArgs = $"system-images;android-{sdkVersion};google_apis_playstore;x86";
            return ExecuteInternal(o => o.Add($"--install \"{installArgs}\""), cancellationToken);
        }

        public static Task InstallLatestCommandLineTools(CancellationToken cancellationToken)
        {
            return ExecuteInternal(o => o.Add(@"--install ""cmdline-tools;latest"""), cancellationToken);
        }

        private static async Task ExecuteInternal(Action<ArgumentsBuilder> configure, CancellationToken cancellationToken)
        {
            ThrowIfNull(ToolPath, nameof(SdkManager));

            var stdErrBuffer = new StringBuilder();
            await Cli.Wrap(ToolPath)
               .WithArguments(configure)
               .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
               .WithStandardOutputPipe(PipeTarget.ToDelegate(l => Logger.WriteLine(l, LogLevel.Detailed)))
               .WithStandardInputPipe(PipeSource.FromString("y"))
               .ExecuteAsync(cancellationToken);

            var stdErr = stdErrBuffer.ToString().Trim();
            if (!string.IsNullOrEmpty(stdErr))
                throw new Exception(stdErr);
        }
    }
}
