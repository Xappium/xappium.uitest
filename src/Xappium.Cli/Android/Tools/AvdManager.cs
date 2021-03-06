using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Builders;
using Xappium.Logging;
using static Xappium.Android.AndroidTool;

namespace Xappium.Android
{
    internal static class AvdManager
    {
        public static string ToolPath => LocateUtility("avdmanager");

        public const string DefaultUITestEmulatorName = "uitest_android_emulator";

        public static async Task InstallEmulator(int sdkVersion, CancellationToken cancellationToken)
        {
            var devices = await GetDevices(cancellationToken);
            var device = devices
                .Where(x => Regex.IsMatch(x, @"^pixel_\d$"))
                .OrderByDescending(x => x)
                .FirstOrDefault();

            Logger.WriteLine($"Installing Emulator for SDK Version: {sdkVersion} with Device {device}", LogLevel.Normal);

            await ExecuteInternal(b =>
            {
                b.Add("create")
                .Add("avd")
                .Add($"-n {DefaultUITestEmulatorName}")
                .Add($@"-k ""system-images;android-{sdkVersion};google_apis_playstore;x86""")
                .Add($"--device {device}")
                .Add("--force");
            }, cancellationToken, PipeSource.FromString("no")).ConfigureAwait(false);
        }

        public static Task DeleteEmulator(CancellationToken cancellationToken)
        {
            return ExecuteInternal(b =>
            {
                b.Add("delete")
                .Add($"-n {DefaultUITestEmulatorName}");
            }, cancellationToken);
        }

        public static async Task<IEnumerable<string>> GetDevices(CancellationToken cancellationToken)
        {
            var output = await ExecuteInternal(b =>
            {
                b.Add("list")
                .Add("device")
                .Add("-c");
            }, cancellationToken).ConfigureAwait(false);

            // Skip the Parsing notification
            return output.Split(Environment.NewLine).Skip(1);
        }

        private static async Task<string> ExecuteInternal(Action<ArgumentsBuilder> configure, CancellationToken cancellationToken, PipeSource stdInput = null)
        {
            var toolPath = ToolPath;
            ThrowIfNull(toolPath, nameof(AvdManager));
            var builder = new ArgumentsBuilder();
            configure(builder);
            var args = builder.Build();
            Logger.WriteLine($"{toolPath} {args}", LogLevel.Normal);
            var stdErrBuffer = new StringBuilder();
            var stdOutBuffer = new StringBuilder();
            var stdOut = PipeTarget.Merge(PipeTarget.ToStringBuilder(stdOutBuffer),
                PipeTarget.ToDelegate(l => Logger.WriteLine(l, LogLevel.Verbose)));

            var cmd = Cli.Wrap(toolPath)
                .WithArguments(args)
                .WithValidation(CommandResultValidation.None)
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .WithStandardOutputPipe(stdOut);

            if (stdInput != null)
                cmd = cmd.WithStandardInputPipe(stdInput);

            await cmd.ExecuteAsync(cancellationToken);

            var stdErr = stdErrBuffer.ToString().Trim();
            if (!string.IsNullOrEmpty(stdErr))
                throw new Exception(stdErr);

            return stdOutBuffer.ToString().Trim();
        }
    }
}
