using FluentAssertions;
using Xappium.Tools;
using Xunit;

namespace Xappium.Cli.Tests.Tools
{
    [Collection(nameof(Tool))]
    public class NodeTests
    {
        [Fact]
        public void NodeIsInstalled()
        {
            Node.IsInstalled.Should().BeTrue();
        }

        [Fact]
        public void NodeVersionIsNotNull()
        {
            Node.Version.Should().NotBeNullOrEmpty();
        }
    }
}
