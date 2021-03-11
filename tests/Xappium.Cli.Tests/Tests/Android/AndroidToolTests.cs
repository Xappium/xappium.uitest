using Xappium.Android;
using Xunit;

namespace Xappium.Cli.Tests.Android
{
    public class AndroidToolTests
    {
        [Theory]
        [InlineData("sdkmanager")]
        [InlineData("adb")]
        [InlineData("avdmanager")]
        [InlineData("emulator")]
        public void LocatesCliTool(string toolName)
        {
            var path = AndroidTool.LocateUtility(toolName);
            Assert.False(string.IsNullOrEmpty(path));
        }
    }
}
