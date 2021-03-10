using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Builders;
using Xappium.Extensions;
using Xappium.Logging;
using static Xappium.Android.AndroidTool;

namespace Xappium.Android
{
    internal static class Emulator
    {
        public static string ToolPath => LocateUtility("emulator");

        public static async Task StartEmulator(CancellationToken cancellationToken)
        {
            var emulators = await ListEmulators(cancellationToken).ConfigureAwait(false);
            await StartEmulator(emulators.FirstOrDefault(), cancellationToken).ConfigureAwait(false);
        }

        public static async Task StartEmulator(string emulatorName, CancellationToken cancellationToken)
        {
            await ValidateStartEmulator(emulatorName, cancellationToken).ConfigureAwait(false);

            // -no-window
#pragma warning disable CS4014 // Launch Emulator without waiting
            using var _ = new Process
            {
                StartInfo = new ProcessStartInfo(ToolPath, $"-avd {emulatorName} -no-snapshot -wipe-data")
                {
                    CreateNoWindow = true,
                    RedirectStandardOutput = true
                }
            };
            _.Start();
#pragma warning restore CS4014 // Launch Emulator without waiting

            // Wait for boot
            await WaitForBoot(cancellationToken).ConfigureAwait(false);

            Logger.WriteLine("Giving the device 20 seconds to finish getting ready.", LogLevel.Minimal);
            await Task.Delay(TimeSpan.FromSeconds(20)).ConfigureAwait(false);
        }

        private static async Task WaitForBoot(CancellationToken cancellationToken)
        {
            var i = 0;
            while (true)
            {
                var result = await Adb.ExecuteInternal(b =>
                {
                    b.Add("wait-for-device")
                    .Add("shell")
                    .Add("getprop")
                    .Add("sys.boot_completed");
                }, cancellationToken).ConfigureAwait(false);

                if(result.Contains("1"))
                {
                    // Wait a few seconds to ensure the OS has fully loaded
                    Logger.WriteLine("Emulator Started... waiting 10 seconds for full boot", LogLevel.Minimal);
                    await Task.Delay(TimeSpan.FromSeconds(10));
                    return;
                }

                if (i++ > 180)
                {
                    throw new TimeoutException($"The Emulator does not appear to have started after 3 minutes");
                }

                await Task.Delay(TimeSpan.FromSeconds(1));
            }
        }

        //private static void EnsureDeviceIsReady()
        //{
        //    var i = 0;
        //    while (true)
        //    {
        //        var result = ProcessHelper.Run(Adb.ToolPath, "wait-for-device shell input keyevent 82");

        //        if (result.IsErred)
        //            throw new Exception(result.Error);

        //        if (result.Output.Any(x => !string.IsNullOrEmpty(x)))
        //            return;

        //        if (i++ > 60)
        //            throw new TimeoutException("The Emulator is not ready after waiting 60 seconds from the boot completion.");

        //        Thread.Sleep(TimeSpan.FromSeconds(1));
        //    }
        //}

        private static async Task ValidateStartEmulator(string emulatorName, CancellationToken cancellationToken)
        {
            ThrowIfNull(Adb.ToolPath, nameof(Adb));

            if (string.IsNullOrEmpty(emulatorName))
            {
                throw new Exception($"No Installed Emulator could be found");
            }

            var emulators = await ListEmulators(cancellationToken);
            if (!emulators.Any(x => x == emulatorName))
            {
                throw new Exception($"No such emulator exists with the name: '{emulatorName}'");
            }
        }

        public static async Task<bool> HasInstalledEmulator(CancellationToken cancellationToken)
        {
            ThrowIfNull(ToolPath, nameof(Emulator));

            try
            {
                var emulators = await ListEmulators(cancellationToken).ConfigureAwait(false);
                return emulators != null && emulators.Any();
            }
            catch
            {
                return false;
            }
        }

        public static async Task<IEnumerable<string>> ListEmulators(CancellationToken cancellationToken)
        {
            var output = await ExecuteInternal(b => b.Add("-list-avds"), cancellationToken).ConfigureAwait(false);
            return output.Split('\n').Select(x => x.Trim()).Where(x => !string.IsNullOrEmpty(x));
        }

        internal static async Task<string> ExecuteInternal(Action<ArgumentsBuilder> configure, CancellationToken cancellationToken)
        {
            var toolPath = ToolPath;
            ThrowIfNull(toolPath, nameof(Emulator));
            var builder = new ArgumentsBuilder();
            configure(builder);
            var args = builder.Build();
            Logger.WriteLine($"{toolPath} {args}", LogLevel.Normal);
            var stdErrBuffer = new StringBuilder();
            var stdOutBuffer = new StringBuilder();
            var stdOut = PipeTarget.Merge(PipeTarget.ToStringBuilder(stdOutBuffer),
                PipeTarget.ToDelegate(l => Logger.WriteLine(l, LogLevel.Verbose)));

            await Cli.Wrap(toolPath)
                .WithArguments(args)
                .WithValidation(CommandResultValidation.None)
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .WithStandardOutputPipe(stdOut)
                .ExecuteAsync(cancellationToken);

            var stdErr = stdErrBuffer.ToString().Trim();
            if (!string.IsNullOrEmpty(stdErr))
                throw new Exception(stdErr);

            return stdOutBuffer.ToString().Trim();
        }
    }
}
