using System.Threading.Tasks;
using Xappium.Android;
using Xunit;

namespace Xappium.Cli.Tests.Android
{
    public class SdkManagerTests
    {
        [Fact]
        public async Task SdkManagerIsInvokable()
        {
            var ex = await Record.ExceptionAsync(() => SdkManager.ExecuteInternal(b => b.Add("--version"), default));
        }
    }
}
