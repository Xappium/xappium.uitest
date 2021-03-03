using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;

namespace Xappium.Tools
{
    internal static class MSBuild
    {
        public static async Task Build(string projectPath, string baseWorkingDirectory, IDictionary<string, string> props, CancellationToken cancellationToken, string target = null)
        {
            if (!props.ContainsKey("Configuration"))
                props.Add("Configuration", "Release");

            if (!props.ContainsKey("Verbosity"))
                props.Add("Verbosity", "Minimal");

            var stdErrBuffer = new StringBuilder();
            var result = await Cli.Wrap("msbuild")
                .WithArguments(b =>
                {
                    b.Add(projectPath);

                    b.Add("/r");

                    if (!string.IsNullOrEmpty(target))
                        b.Add($"/t:{target}");

                    foreach ((var key, var value) in props)
                    {
                        b.Add($"/p:{key}={value}");
                    }

                    var logoutput = Path.Combine(baseWorkingDirectory, "logs", $"{Path.GetFileNameWithoutExtension(projectPath)}.binlog");
                    b.Add($"/bl:{logoutput}");
                })
                .WithStandardOutputPipe(PipeTarget.ToDelegate(l => Console.WriteLine(l)))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .WithValidation(CommandResultValidation.None)
                .ExecuteAsync(cancellationToken)
                .ConfigureAwait(false);

            var error = stdErrBuffer.ToString().Trim();
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);
        }
    }
}
