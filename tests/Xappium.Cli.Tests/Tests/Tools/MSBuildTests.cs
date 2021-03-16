using FluentAssertions;
using Xappium.Tools;
using Xunit;

namespace Xappium.Cli.Tests.Tools
{
    public class MSBuildTests
    {
        [Fact]
        public void LocatesMSBuildPath()
        {
            MSBuild.ToolPath.Should().NotBeNullOrEmpty();
        }
    }
}
