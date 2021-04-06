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

        public const string DefaultUITestEmulatorName = "xappium_emulator_sdk";

        public static async Task InstallEmulator(string emulatorName, int sdkVersion, CancellationToken cancellationToken)
        {
            var devices = await GetDevices(cancellationToken);
            var device = devices
                .Where(x => Regex.IsMatch(x, @"^pixel_\d$"))
                .OrderByDescending(x => x)
                .FirstOrDefault();

            if (string.IsNullOrEmpty(device))
            {
                Logger.WriteError("Current Devices");
                foreach (var d in devices)
                    Logger.WriteError($"  - {d}");

                throw new Exception("No pixel device found.");
            }

            Logger.WriteLine($"Installing Emulator for SDK Version: {sdkVersion} with Device {device}", LogLevel.Normal);

            await ExecuteInternal(b =>
            {
                b.Add("create")
                .Add("avd")
                .Add("--name")
                .Add(emulatorName)
                .Add("--package")
                .Add($"system-images;android-{sdkVersion};google_apis_playstore;x86")
                .Add("--device")
                .Add(device)
                .Add("--force");
            }, cancellationToken, PipeSource.FromString("no")).ConfigureAwait(false);
        }

        public static Task DeleteEmulator(string emulatorName, CancellationToken cancellationToken)
        {
            return ExecuteInternal(b =>
            {
                b.Add("delete")
                 .Add("avd")
                 .Add("--name")
                 .Add(emulatorName);
            }, cancellationToken);
        }

        public static async Task<IEnumerable<string>> GetDevices(CancellationToken cancellationToken)
        {
            var output = await ExecuteInternal(b =>
            {
                b.Add("list")
                .Add("device")
                .Add("--compact");
            }, cancellationToken).ConfigureAwait(false);

            // Skip the Parsing notification
            return output.Split('\n').Select(x => x.Trim()).Skip(1);
        }

        private static async Task<string> ExecuteInternal(Action<ArgumentsBuilder> configure, CancellationToken cancellationToken, PipeSource stdInput = null)
        {
            var toolPath = ToolPath;
            ThrowIfNull(toolPath, nameof(AvdManager));
            var builder = new ArgumentsBuilder();
            configure(builder);
            var args = builder.Build();
            Logger.WriteLine($"{toolPath} {args}", LogLevel.Normal);
            var errorBuffer = new List<string>();
            var stdOutBuffer = new StringBuilder();
            var stdOut = PipeTarget.Merge(PipeTarget.ToStringBuilder(stdOutBuffer),
                PipeTarget.ToDelegate(l => Logger.WriteLine(l, LogLevel.Verbose)));

            var stdErr = PipeTarget.ToDelegate(l =>
            {
                if (string.IsNullOrEmpty(l))
                    return;
                else if (l.Contains("Warning: "))
                    Logger.WriteWarning(l);
                else
                    errorBuffer.Add(l);
            });

            var cmd = Cli.Wrap(toolPath)
                .WithArguments(args)
                .WithValidation(CommandResultValidation.None)
                .WithStandardErrorPipe(stdErr)
                .WithStandardOutputPipe(stdOut);

            if (stdInput != null)
                cmd = cmd.WithStandardInputPipe(stdInput);

            await cmd.ExecuteAsync(cancellationToken);

            if (errorBuffer.Any())
                throw new Exception(string.Join(Environment.NewLine, errorBuffer));

            return stdOutBuffer.ToString().Trim();
        }
    }
}
