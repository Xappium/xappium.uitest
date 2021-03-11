using System;
using TestApp.UITests.Pages;
using Xappium.UITest;
using Xunit;

namespace TestApp.UITests
{
    public class AppTests : IDisposable
    {
        private ITestEngine Engine { get; }

        public AppTests()
        {
            AppManager.StartApp();
            Engine = AppManager.Engine;
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

        public void Dispose()
        {
            Engine.StopApp();
        }
    }
}
