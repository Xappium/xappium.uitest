using System.Threading.Tasks;
using FluentAssertions;
using Xappium.Tools;
using Xunit;

namespace Xappium.Cli.Tests.Tools
{
    [Collection(nameof(Tool))]
    public class GemTests
    {
        public GemTests()
        {
            TestEnvironmentHost.Init();
        }

        [MacOSFact]
        public async Task InstallsXcPretty()
        {
            var ex = await Record.ExceptionAsync(() => Gem.InstallXcPretty(default));
            ex.Should().BeNull();
        }
    }
}