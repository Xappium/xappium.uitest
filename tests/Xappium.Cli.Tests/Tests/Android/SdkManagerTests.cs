using System.Threading.Tasks;
using Xappium.Android;
using Xappium.Cli.Tests.Tools;
using Xunit;

namespace Xappium.Cli.Tests.Android
{
    [Collection(nameof(Tool))]
    public class SdkManagerTests
    {
        [Fact]
        public async Task SdkManagerIsInvokable()
        {
            var ex = await Record.ExceptionAsync(() => SdkManager.ExecuteInternal(b => b.Add("--version"), default));
            Assert.Null(ex);
        }

        [Fact]
        public async Task InstallsWebDriver()
        {
            var ex = await Record.ExceptionAsync(() => SdkManager.InstallWebDriver(default));
            Assert.Null(ex);
        }

        [Fact]
        public async Task InstallsLatestCliTools()
        {
            var ex = await Record.ExceptionAsync(() => SdkManager.InstallLatestCommandLineTools(default));
            Assert.Null(ex);
        }

        [Theory]
        [InlineData(28)]
        [InlineData(29)]
        [InlineData(30)]
        public async Task InstallAndroidSdk(int sdkLevel)
        {
            var ex = await Record.ExceptionAsync(() => SdkManager.EnsureSdkIsInstalled(sdkLevel, default));
            Assert.Null(ex);
        }
    }
}
