using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using McMaster.Extensions.CommandLineUtils;
using Xappium.Logging;
using Xappium.Tools;
using Xappium.Utilities;

namespace Xappium.Commands
{
    [Command(Description = "Prepares the tests, then ensures Appium is installed, starts Appium, and runs Tests.")]
    public class TestCommand : CliBase
    {
        [Option(Description = "Skips running the appium server as part of the tool and assumes another running instance",
            LongName = "skip-appium",
            ShortName = "sa")]
        public bool SkipAppium { get; } = false;

        [Option(Description = "Specifies the address to start appium server listening on.",
            LongName = "appium-address",
            ShortName = "aa")]
        public string AppiumAddress { get; } = "127.0.0.1";

        [Option(Description = "Specifies the port to start appium server listening on.",
            LongName = "appium-port",
            ShortName = "ap")]
        public int AppiumPort { get; } = 4723;

        protected override async Task OnExecuteInternal(CancellationToken cancellationToken)
        {
            if (AppiumPort < 80 || AppiumPort > ushort.MaxValue)
                throw new Exception("Specified Appium Port is out of range");

            if (Uri.CheckHostName(AppiumAddress) == UriHostNameType.Unknown)
                throw new Exception("Invalid Appium Address specified.  Must by IP Address or valid host name.");

            if (!Node.IsInstalled)
                throw new Exception("Your environment does not appear to have Node installed. This is required to run Appium");

            var disposable = new DelegateDisposable(() =>
            {
                var binDir = Path.Combine(BaseWorkingDirectory, "bin");
                if (Directory.Exists(binDir))
                    Directory.Delete(binDir, true);
            });
            Disposables.Add(disposable);

            await PrepareProjects(cancellationToken);

            Appium.Address = AppiumAddress;
            Appium.Port = AppiumPort;

            Logger.WriteLine($"Appium {AppiumAddress}:{AppiumPort}", LogLevel.Normal);

            if (!SkipAppium)
            {
                Logger.WriteLine($"Installing/running Appium...", LogLevel.Normal);

                if (!await Appium.Install(cancellationToken))
                    return;

                var appium = await Appium.Run(BaseWorkingDirectory).ConfigureAwait(false);
                Disposables.Add(appium);
            }
            else
            {
                Logger.WriteLine("Appium skipped.", LogLevel.Normal);
            }

            await DotNetTool.Test(UITestProjectPathInfo.FullName, UiTestBin, Configuration?.Trim(), Path.Combine(BaseWorkingDirectory, "results"), cancellationToken)
                .ConfigureAwait(false);
        }
    }
}
