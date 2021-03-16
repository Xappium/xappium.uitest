using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Xappium.BuildSystem;
using Xappium.Configuration;
using Xappium.Logging;
using Xappium.Tools;

namespace Xappium
{
    internal class Program
    {
        private FileInfo UITestProjectPathInfo => string.IsNullOrEmpty(UITestProjectPath) ? null : new FileInfo(UITestProjectPath);

        private FileInfo DeviceProjectPathInfo => string.IsNullOrEmpty(DeviceProjectPath) ? null : new FileInfo(DeviceProjectPath);

        public static Task<int> Main(string[] args)
            => CommandLineApplication.ExecuteAsync<Program>(args);

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

        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members",
            Justification = "Called by McMaster")]
        private async Task<int> OnExecuteAsync(CancellationToken cancellationToken)
        {
            IDisposable appium = null;
            Logger.Level = LogLevel;

            if (Directory.Exists(BaseWorkingDirectory))
                Directory.Delete(BaseWorkingDirectory, true);

            Directory.CreateDirectory(BaseWorkingDirectory);
            Logger.SetWorkingDirectory(BaseWorkingDirectory);

            try
            {
                if (!Node.IsInstalled)
                    throw new Exception("Your environment does not appear to have Node installed. This is required to run Appium");

                Logger.WriteLine($"Build and Test artifacts will be stored at {BaseWorkingDirectory}", LogLevel.Detailed);

                ValidatePaths();

                var headBin = Path.Combine(BaseWorkingDirectory, "bin", "device");
                var uiTestBin = Path.Combine(BaseWorkingDirectory, "bin", "uitest");

                Directory.CreateDirectory(headBin);
                Directory.CreateDirectory(uiTestBin);

                // HACK: The iOS SDK will mess up the generated app output if a Separator is not at the end of the path.
                headBin += Path.DirectorySeparatorChar;
                uiTestBin += Path.DirectorySeparatorChar;

                var appProject = CSProjFile.Load(DeviceProjectPathInfo, new DirectoryInfo(headBin), Platform);
                if (!await appProject.IsSupported())
                    throw new PlatformNotSupportedException($"{appProject.Platform} is not supported on this machine. Please check that you have the correct build dependencies.");

                await appProject.Build(Configuration, cancellationToken).ConfigureAwait(false);

                var uitestProj = CSProjFile.Load(UITestProjectPathInfo, new DirectoryInfo(uiTestBin), string.Empty);
                await uitestProj.Build(Configuration, cancellationToken).ConfigureAwait(false);

                if (cancellationToken.IsCancellationRequested)
                    return 0;

                await ConfigurationGenerator.GenerateTestConfig(headBin, uiTestBin, appProject.Platform, ConfigurationPath, BaseWorkingDirectory, AndroidSdk, DisplayGeneratedConfig, cancellationToken);

                if(!await Appium.Install(cancellationToken))
                {
                    return 0;
                }

                appium = await Appium.Run(BaseWorkingDirectory).ConfigureAwait(false);

                await DotNetTool.Test(UITestProjectPathInfo.FullName, uiTestBin, Configuration?.Trim(), Path.Combine(BaseWorkingDirectory, "results"), cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                Logger.WriteError(ex.Message);
                return 1;
            }
            finally
            {
                appium?.Dispose();

                var binDir = Path.Combine(BaseWorkingDirectory, "bin");
                if(Directory.Exists(binDir))
                   Directory.Delete(binDir, true);
            }

            return Logger.HasErrors ? 1 : 0;
        }

        private void ValidatePaths()
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
    }
}
