using System.Threading;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;

namespace Xappium.Commands
{
    [Command(Description = "Builds the UI Test and App and prepares the configuration for the test run")]
    public class PrepareCommand : CliBase
    {
        protected override Task OnExecuteInternal(CancellationToken cancellationToken) => PrepareProjects(cancellationToken);
    }
}
