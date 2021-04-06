using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xappium.Apple;
using Xappium.Tools;

namespace Xappium.BuildSystem
{
    [SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "iOS is named correctly")]
    internal class iOSProjectFile : CSProjFile
    {
        public iOSProjectFile(FileInfo projectFile, DirectoryInfo outputDirectory)
            : base(projectFile, outputDirectory)
        {
        }

        public override string Platform => "iOS";

        public override async Task Build(string configuration, CancellationToken cancellationToken)
        {
            var outputPath = OutputDirectory.FullName + Path.DirectorySeparatorChar;
            var props = new Dictionary<string, string>
            {
                { "OutputPath", outputPath },
                { "Configuration", string.IsNullOrEmpty(configuration) ? "Release" : configuration },
                { "Platform", "iPhoneSimulator" }
            };

            // msbuild ../sample/TestApp.iOS/TestApp.iOS.csproj /p:Platform=iPhoneSimulator /p:Configuration=Release /p:OutputPath=$UITESTPATH/bin/
            await MSBuild.Build(ProjectFile.FullName, OutputDirectory.Parent.Parent.FullName, props, cancellationToken).ConfigureAwait(false);
        }

        public override Task<bool> IsSupported()
        {
            AppleDeviceInfo simulator = null;
            try
            {
                simulator = AppleSimulator.GetSimulator();
            }
            catch(Exception ex)
            {
                Logging.Logger.WriteError(ex);
            }

            return Task.FromResult(simulator != null);
        }
    }
}
