using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using FluentAssertions;
using Xappium.Tools;
using Xunit;

namespace Xappium.Cli.Tests.Tools
{
    public class AppiumTests
    {
        public AppiumTests()
        {
            TestEnvironmentHost.Init();
        }

        [Fact]
        public async Task InstallsAppium()
        {
            Assert.True(await Appium.Install(default));
        }

        [Fact]
        public async Task RunsAppium()
        {
            var path = Path.Combine(TestEnvironmentHost.BaseWorkingDirectory, nameof(RunsAppium));
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            // Ensure Appium is installed for test...
            await Appium.Install(default);

            IDisposable disposable = null;
            try
            {
                var ex = await Record.ExceptionAsync(async () => disposable = await Appium.Run(path));

                ex.Should().BeNull();
                disposable.Should().NotBeNull();

                await AssertAppiumIsRunning(true);
                disposable.Dispose();
                disposable = null;
                ex = await Record.ExceptionAsync(() => AssertAppiumIsRunning(false));
                ex.Should().NotBeNull().And.BeOfType<HttpRequestException>();
                ex.Message.Should().BeSameAs("Connection refused");
            }
            finally
            {
                disposable?.Dispose();
            }
        }

        private async Task AssertAppiumIsRunning(bool isRunning)
        {
            await Task.Delay(1500);
            using var client = new HttpClient();
            using var respose = await client.GetAsync("http://127.0.0.1:4723/wd/hub/status");

            respose.IsSuccessStatusCode.Should().Be(isRunning);
        }
    }
}
