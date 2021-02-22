using System;
using Xunit;
using Xappium.Android;

namespace Xappium.Cli.Tests
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
