using System;
using System.IO;
using System.Threading.Tasks;
using System.Threading;
using McMaster.Extensions.CommandLineUtils;
using Xappium.Logging;
using Xappium.BuildSystem;
using Xappium.Configuration;
using System.Reactive.Disposables;

namespace Xappium.Commands
{
    public abstract class CliBase
    {
        protected readonly CompositeDisposable Disposables = new CompositeDisposable();

        protected FileInfo UITestProjectPathInfo => string.IsNullOrEmpty(UITestProjectPath) ? null : new FileInfo(UITestProjectPath);

        protected string HeadBin
        {
            get
            {
                var headBin = Path.Combine(BaseWorkingDirectory, "bin", "device");
                // HACK: The iOS SDK will mess up the generated app output if a Separator is not at the end of the path.
                headBin += Path.DirectorySeparatorChar;
                return headBin;
            }
        }

        protected string UiTestBin
        {
            get
            {
                var uiTestBin = Path.Combine(BaseWorkingDirectory, "bin", "uitest");
                // HACK: The iOS SDK will mess up the generated app output if a Separator is not at the end of the path.
                uiTestBin += Path.DirectorySeparatorChar;
                return uiTestBin;
            }
        }

        protected FileInfo DeviceProjectPathInfo => string.IsNullOrEmpty(DeviceProjectPath) ? null : new FileInfo(DeviceProjectPath);

        [Option(Description = "Specifies the csproj path of the UI Test project",
            LongName = "uitest-project-path",
            ShortName = "uitest")]
        public string UITestProjectPath { get; }

        [Option(Description = "Specifies the Head Project csproj path for your iOS or Android project.",
            LongName = "app-project-path",
            ShortName = "app")]
        public string DeviceProjectPath { get; }

        [Option(Description = "Specifies the target platform such as iOS or Android.",
            LongName = "platform",
            ShortName = "p")]
        public string Platform { get; }

        [Option(Description = "Specifies the Build Configuration to use on the Platform head and UITest project",
            LongName = "configuration",
            ShortName = "c")]
        public string Configuration { get; } = "Release";

        [Option(Description = "Specifies a UITest.json configuration path that overrides what may be in the UITest project build output directory",
            LongName = "uitest-configuration",
            ShortName = "ui-config")]
        public string ConfigurationPath { get; }

        [Option(Description = "Specifies the test artifact folder",
            LongName = "artifact-staging-directory",
            ShortName = "artifacts")]
        public string BaseWorkingDirectory { get; } = Path.Combine(Environment.CurrentDirectory, "UITest");

        [Option(Description = "Will write the generated uitest.json to the console. This should only be done if you do not have sensative settings that may be written to the console.",
            LongName = "show-config",
            ShortName = "show")]
        public bool DisplayGeneratedConfig { get; }

        [Option(Description = "Sets the level of logging output to the console.",
            LongName = "logger",
            ShortName = "l")]
        public LogLevel LogLevel { get; } = LogLevel.Normal;

        [Option(Description = "Specifies the Android SDK version to ensure is installed for the Emulator",
            LongName = "android-sdk",
            ShortName = "droid")]
        public int? AndroidSdk { get; }

        public async Task<int> OnExecuteAsync(CancellationToken cancellationToken)
        {
            Logger.Level = LogLevel;

            if (Directory.Exists(BaseWorkingDirectory))
                Directory.Delete(BaseWorkingDirectory, true);

            Directory.CreateDirectory(BaseWorkingDirectory);
            Logger.SetWorkingDirectory(BaseWorkingDirectory);

            try
            {
                Logger.WriteLine($"Build and Test artifacts will be stored at {BaseWorkingDirectory}", LogLevel.Detailed);

                ValidatePaths();

                await OnExecuteInternal(cancellationToken);
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.Message);
                return 1;
            }
            finally
            {
                Disposables.Dispose();
            }

            return Logger.HasErrors ? 1 : 0;
        }

        protected abstract Task OnExecuteInternal(CancellationToken cancellationToken);

        protected void ValidatePaths()
        {
            if (UITestProjectPathInfo.Extension != ".csproj")
                throw new InvalidOperationException($"The path '{UITestProjectPath}' does not specify a valid csproj");
            else if (DeviceProjectPathInfo.Extension != ".csproj")
                throw new InvalidOperationException($"The path '{DeviceProjectPath}' does not specify a valid csproj");
            else if (!UITestProjectPathInfo.Exists)
                throw new FileNotFoundException($"The specified UI Test project path does not exist: '{UITestProjectPath}'");
            else if (!DeviceProjectPathInfo.Exists)
                throw new FileNotFoundException($"The specified Platform head project path does not exist: '{DeviceProjectPath}'");
        }

        protected async Task PrepareProjects(CancellationToken cancellationToken)
        {
            Directory.CreateDirectory(HeadBin);
            Directory.CreateDirectory(UiTestBin);

            var appProject = CSProjFile.Load(DeviceProjectPathInfo, new DirectoryInfo(HeadBin), Platform);
            if (!await appProject.IsSupported())
                throw new PlatformNotSupportedException($"{appProject.Platform} is not supported on this machine. Please check that you have the correct build dependencies.");

            await appProject.Build(Configuration, cancellationToken).ConfigureAwait(false);

            var uitestProj = CSProjFile.Load(UITestProjectPathInfo, new DirectoryInfo(UiTestBin), string.Empty);
            await uitestProj.Build(Configuration, cancellationToken).ConfigureAwait(false);

            if (cancellationToken.IsCancellationRequested)
                return;

            await ConfigurationGenerator.GenerateTestConfig(HeadBin, UiTestBin, appProject.Platform, ConfigurationPath, BaseWorkingDirectory, AndroidSdk, DisplayGeneratedConfig, cancellationToken);
        }
    }
}
