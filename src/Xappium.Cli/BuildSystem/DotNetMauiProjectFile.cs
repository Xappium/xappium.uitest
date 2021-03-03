using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Xappium.Tools;

namespace Xappium.BuildSystem
{
    internal class DotNetMauiProjectFile : CSProjFile
    {
        public DotNetMauiProjectFile(FileInfo projectFile, DirectoryInfo outputDirectory,
            string platform, string targetFramework)
            : base(projectFile, outputDirectory)
        {
            TargetFramework = targetFramework;
            Platform = platform;
        }

        public string TargetFramework { get; }

        public override string Platform { get; }

        public override Task Build(string configuration, CancellationToken cancellationToken)
        {
            if (string.IsNullOrEmpty(configuration))
                configuration = "Release";

            // dotnet build HelloForms -t:Run -f net6.0-ios
            return DotNetTool.Build(b =>
                    b.Add($"{ProjectFile.FullName}")
                     .Add($"--framework={TargetFramework}")
                     .Add($"--output={OutputDirectory.FullName}")
                     .Add($"--configuration={configuration}"), cancellationToken);
        }

        public override Task<bool> IsSupported()
        {
            return (Platform.ToLower()) switch
            {
                "android" => Task.FromResult(EnvironmentHelper.IsAndroidSupported),
                "ios" => Task.FromResult(EnvironmentHelper.IsIOSSupported),
                _ => Task.FromResult(false),
            };
        }
    }
}
