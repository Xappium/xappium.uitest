using TestApp.UITests.Pages;
using Xappium.UITest;
using Xunit;

namespace TestApp.UITests
{
    public class AppTests : IClassFixture<AppFixture>
    {
        private ITestEngine Engine { get; }

        public AppTests(AppFixture fixture)
        {
            Engine = fixture.Engine;
        }

        [Fact]
        public void AppLaunches()
        {
            new LoginPage();
        }

        [Fact]
        public void LogsIntoMainPage()
        {
            new LoginPage()
                .EnterUsername(Engine.Settings["username"])
                .EnterPassword(Engine.Settings["password"])
                .TapLoginButton();

            new MainPage()
                .ValidateWelcomeMessage();
        }
    }
}
