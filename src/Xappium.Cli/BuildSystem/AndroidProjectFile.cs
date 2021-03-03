using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xappium.Tools;

namespace Xappium.BuildSystem
{
    internal class AndroidProjectFile : CSProjFile
    {
        public AndroidProjectFile(FileInfo projectFile, DirectoryInfo outputDirectory)
            : base(projectFile, outputDirectory)
        {
        }

        public override string Platform => "Android";

        public override async Task Build(string configuration, CancellationToken cancellationToken)
        {
            var props = new Dictionary<string, string>
            {
                { "OutputPath", OutputDirectory.FullName },
                { "Configuration", string.IsNullOrEmpty(configuration) ? "Release" : configuration },
                { "AndroidPackageFormat", "apk" },
                { "AndroidSupportedAbis", "x86" }
            };

            // msbuild ../sample/TestApp.Android/TestApp.Android.csproj /p:Configuration=Release /p:AndroidPackageFormat=apk /p:AndroidSupportedAbis=x86 /p:OutputPath=$UITESTPATH/bin/ /t:SignAndroidPackage
            await MSBuild.Build(ProjectFile.FullName, OutputDirectory.Parent.Parent.FullName, props, cancellationToken, "SignAndroidPackage").ConfigureAwait(false);
        }

        public override Task<bool> IsSupported() =>
            Task.FromResult(EnvironmentHelper.IsAndroidSupported);
    }
}
