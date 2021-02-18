using System;
using Xappium.UITest.Pages;

namespace TestApp.UITests.Pages
{
    public class MainPage : BasePage
    {
        public MainPage()
        {
        }

        protected override string Trait => "WelcomeMessage";

        public MainPage ValidateWelcomeMessage()
        {
            Engine.AssertTextInElementByAutomationId("WelcomeMessage", $"Hello {Engine.Settings["username"]}!");
            Engine.Screenshot("Has Welcome message");
            return this;
        }
    }
}
