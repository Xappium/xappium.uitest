using Xappium.Tools;
using Xunit;

namespace Xappium.Cli.Tests.Tools
{
    public class NodeTests
    {
        [Fact]
        public void NodeIsInstalled()
        {
            Assert.True(Node.IsInstalled);
        }

        [Fact]
        public void NodeVersionIsNotNull()
        {
            Assert.False(string.IsNullOrEmpty(Node.Version));
        }
    }
}
