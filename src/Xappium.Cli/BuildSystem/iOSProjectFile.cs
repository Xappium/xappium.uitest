using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Threading.Tasks;
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

        public override async Task Build(string configuration)
        {
            var outputPath = OutputDirectory.FullName + Path.DirectorySeparatorChar;
            var props = new Dictionary<string, string>
            {
                { "OutputPath", outputPath },
                { "Configuration", string.IsNullOrEmpty(configuration) ? "Release" : configuration },
                { "Platform", "iPhoneSimulator" }
            };

            // msbuild ../sample/TestApp.iOS/TestApp.iOS.csproj /p:Platform=iPhoneSimulator /p:Configuration=Release /p:OutputPath=$UITESTPATH/bin/
            await MSBuild.Build(ProjectFile.FullName, OutputDirectory.Parent.Parent.FullName, props).ConfigureAwait(false);
        }
    }
}
