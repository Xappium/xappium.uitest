using System.Threading.Tasks;
using FluentAssertions;
using Xappium.Tools;
using Xunit;

namespace Xappium.Cli.Tests.Tools
{
    [Collection(nameof(Tool))]
    public class PipTests
    {
        public PipTests()
        {
            TestEnvironmentHost.Init();
        }

        [MacOSFact]
        public async Task UpdatesPip()
        {
            var ex = await Record.ExceptionAsync(() => Pip.UpgradePip(default));
            ex.Should().BeNull();
        }

        [MacOSFact]
        public async Task InstallsIdbClient()
        {
            var ex = await Record.ExceptionAsync(() => Pip.InstallIdbClient(default));
            ex.Should().BeNull();
        }
    }
}