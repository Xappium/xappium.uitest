using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xappium.Tools;
using Xunit;

namespace Xappium.Cli.Tests.Tools
{
    public class BrewTests
    {
        public BrewTests()
        {
            TestEnvironmentHost.Init();
        }

        [Fact]
        public async Task InstallsAppleSimUtils()
        {
            var ex = await Record.ExceptionAsync(() => Brew.InstallAppleSimUtils(default));
            Assert.Null(ex);
        }

        [Fact]
        public async Task InstallsFFMPEG()
        {
            var ex = await Record.ExceptionAsync(() => Brew.InstallFFMPEG(default));
            Assert.Null(ex);
        }

        [Fact]
        public async Task InstallsIdbCompanion()
        {
            var ex = await Record.ExceptionAsync(() => Brew.InstallIdbCompanion(default));
            Assert.Null(ex);
        }
    }
}
