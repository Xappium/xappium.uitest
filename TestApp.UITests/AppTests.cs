using Appium.TestHelpers;
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
            Engine.WaitForElement("MainPage");
            Engine.Screenshot("App did Launch");
        }

        [Fact]
        public void EntryUpdates()
        {
            var name = Engine.Settings["name"];
            Engine.WaitForElement("NameEntry");
            Engine.EnterText("NameEntry", name);
            Engine.Screenshot("Name Entered");
            Engine.AssertTextInElementByAutomationId("WelcomeMessage", $"Hello {name}!");
        }
    }
}
