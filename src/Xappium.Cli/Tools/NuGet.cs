using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using CliWrap;

namespace Xappium.Tools
{
    public static class NuGet
    {
        public static async Task Restore(string path, CancellationToken cancellationToken)
        {
            Console.WriteLine($"Restoring Project: {path}");
            var stdErrBuffer = new StringBuilder();
            await Cli.Wrap("nuget")
                .WithArguments(b =>
                {
                    b.Add("restore")
                    .Add(path);
                })
                .WithStandardOutputPipe(PipeTarget.ToDelegate(l => Console.WriteLine(l)))
                .WithStandardErrorPipe(PipeTarget.ToStringBuilder(stdErrBuffer))
                .ExecuteAsync(cancellationToken)
                .ConfigureAwait(false);

            var error = stdErrBuffer.ToString().Trim();
            if (!string.IsNullOrEmpty(error))
                throw new Exception(error);
        }
    }
}
