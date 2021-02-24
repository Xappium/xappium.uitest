using System;
using Xappium.UITest;
using Xappium.UITest.Pages;
using Xunit;

namespace TestApp.UITests.Pages
{
    public class LoginPage : BasePage
    {
        protected override string Trait => "UsernameEntry";

        public LoginPage EnterUsername(string username)
        {
            var element = Engine.EnterText("UsernameEntry", username);
            Engine.Screenshot($"Entered username {username}");
            Assert.Equal(username, element.Text);
            return this;
        }

        public LoginPage EnterPassword(string password)
        {
            var element = Engine.EnterText("PasswordEntry", password);
            Engine.Screenshot("EnteredPassword");
            return this;
        }

        public void TapLoginButton()
        {
            Engine.Tap("LoginButton");
            Engine.Screenshot("Tapped Login Button");
        }
    }
}
