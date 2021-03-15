using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xappium.Tools;
using Xunit;

namespace Xappium.Cli.Tests.Tools
{
    public class GemTests
    {
        public GemTests()
        {
            TestEnvironmentHost.Init();
        }

        [Fact]
        public async Task InstallsXcPretty()
        {
            var ex = await Record.ExceptionAsync(() => Gem.InstallXcPretty(default));

            Assert.Null(ex);
        }
    }
}