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
        public static Task Test(string projectPath, string outputPath, string configuration, string resultsDirectory)
        {
            if (string.IsNullOrEmpty(configuration))
                configuration = "Release";

            var args = new ArgumentsBuilder().Add("test")
                     .Add($"{projectPath}")
                     .Add($"--output={outputPath}")
                     .Add($"--configuration={configuration}")
                     .Add("--no-build")
                     .Add($"--results-directory={resultsDirectory}")
                     .Add("--logger=trx")
                     .Add("--diag:logs/log.txt")
                     //.Add("--verbosity=diagnostic")
                     .Build();

            Console.WriteLine($"Running dotnet test on '{projectPath}'");
            Console.WriteLine($"{DotNetExe.FullPath} {args}");

            return Execute(args);
        }

        public static Task Build(Action<ArgumentsBuilder> configure)
        {
            var builder = new ArgumentsBuilder()
                .Add("build");
            configure(builder);

            return Execute(builder.Build());
        }

        private static async Task Execute(string args)
        {
            var stdErrBuffer = new StringBuilder();
            var stdOut = PipeTarget.ToDelegate(l => Console.WriteLine(l));
            var stdErr = PipeTarget.Merge(
                PipeTarget.ToStringBuilder(stdErrBuffer),
                PipeTarget.ToDelegate(l => Console.WriteLine(l)));

            var result = await Cli.Wrap(DotNetExe.FullPath)
                .WithArguments(args)
                .WithStandardErrorPipe(stdOut)
                .WithStandardErrorPipe(stdErr)
                .WithValidation(CommandResultValidation.None)
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
