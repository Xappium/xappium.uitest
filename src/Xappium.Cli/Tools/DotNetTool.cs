using System;
using System.Text;
using System.Threading.Tasks;
using CliWrap;
using CliWrap.Builders;
using McMaster.Extensions.CommandLineUtils;

namespace Xappium.Tools
{
    internal static class DotNetTool
    {
        public static async Task Test(string projectPath, string outputPath, string configuration, string resultsDirectory)
        {
            if (string.IsNullOrEmpty(configuration))
                configuration = "Release";

            var stdErrBuffer = new StringBuilder();
            var stdOut = PipeTarget.ToDelegate(l => Console.WriteLine(l));
            var stdErr = PipeTarget.Merge(
                PipeTarget.ToStringBuilder(stdErrBuffer),
                PipeTarget.ToDelegate(l => Console.WriteLine(l)));

            var args = new ArgumentsBuilder().Add("test")
                     .Add($"{projectPath}")
                     .Add($"--output={outputPath}")
                     .Add($"--configuration={configuration}")
                     .Add("--no-build")
                     .Add($"--results-directory={resultsDirectory}")
                     .Add("--logger=trx")
                     .Build();

            Console.WriteLine($"Running dotnet test on '{projectPath}'");
            Console.WriteLine($"{DotNetExe.FullPath} {args}");
            var result = await Cli.Wrap("dotnet")
                .WithArguments(args)
                .WithStandardErrorPipe(stdOut)
                .WithStandardErrorPipe(stdErr)
                .WithValidation(CommandResultValidation.None)
                .WithWorkingDirectory(outputPath)
                .ExecuteAsync()
                .ConfigureAwait(false);

            var error = stdErrBuffer.ToString().Trim();
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);

            if (result.ExitCode > 0)
                throw new Exception($"The dotnet tool unexpectidly exited without any error output with code: {result.ExitCode}");
        }
    }
}
