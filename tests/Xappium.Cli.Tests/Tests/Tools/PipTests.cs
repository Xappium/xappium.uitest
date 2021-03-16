using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Xappium.Tools;
using Xunit;

namespace Xappium.Cli.Tests.Tools
{
    public class PipTests
    {
        public PipTests()
        {
            TestEnvironmentHost.Init();
        }

        [MacOSFact]
        public async Task InstallsIdbClient()
        {
            var ex = await Record.ExceptionAsync(() => Pip.InstallIdbClient(default));

            Assert.Null(ex);
        }
    }
}