using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
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

            IDisposable disposable = null;
            try
            {
                var ex = await Record.ExceptionAsync(async () => disposable = await Appium.Run(path));
                Assert.Null(ex);
                Assert.NotNull(disposable);

                await AssertAppiumIsRunning(true);
                disposable.Dispose();
                disposable = null;
                ex = await Record.ExceptionAsync(() => AssertAppiumIsRunning(false));
                Assert.NotNull(ex);
                Assert.IsType<HttpRequestException>(ex);
                Assert.Equal("Connection refused", ex.Message);
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
            Assert.Equal(isRunning, respose.IsSuccessStatusCode);
        }
    }
}
