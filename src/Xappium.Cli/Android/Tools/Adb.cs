using System;
using System.Collections.Generic;
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
    internal static class Adb
    {
        private const string androidDeviceRegex = @"\s+(device)$";

        public static string ToolPath => LocateUtility("adb");

        public static async Task<bool> DeviceIsConnected(CancellationToken cancellationToken)
        {
            try
            {
                var devices = await ListDevices(cancellationToken).ConfigureAwait(false);
                return devices.Any();
            }
            catch
            {
                return false;
            }
        }

        public static async Task<IEnumerable<AndroidDevice>> ListDevices(CancellationToken cancellationToken)
        {
            var output = await ExecuteInternal(b => b.Add("devices"), cancellationToken).ConfigureAwait(false);

            var ids = output.Split('\n').Select(x => x.Trim()).Where(x => Regex.IsMatch(x, androidDeviceRegex))
                .Select(x => Regex.Replace(x, androidDeviceRegex, string.Empty));

            // List of devices attached
            var devices = new List<AndroidDevice>();
            foreach (var deviceId in ids)
            {
                output = await ExecuteInternal(b => b.Add("-s")
                                                     .Add(deviceId)
                                                     .Add("shell")
                                                     .Add("group"), cancellationToken);

                devices.Add(new AndroidDevice(deviceId, output.Split('\n').Select(x => x.Trim()).ToArray()));
            }

            if (!devices.Any())
                Logger.WriteLine("-- NO DEVICES FOUND --", LogLevel.Minimal);

            return devices;
        }

        internal static async Task<string> ExecuteInternal(Action<ArgumentsBuilder> configure, CancellationToken cancellationToken)
        {
            var toolPath = ToolPath;
            ThrowIfNull(toolPath, nameof(Adb));
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
