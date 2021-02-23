using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using Xappium.UITest.Configuration;
using Xappium.UITest.Extensions;
using OpenQA.Selenium;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Appium.Android;
using OpenQA.Selenium.Appium.Enums;
using OpenQA.Selenium.Appium.iOS;
using OpenQA.Selenium.Appium.MultiTouch;
using OpenQA.Selenium.Support.UI;
using System.Reflection;
using Xappium.UITest.Pages;

namespace Xappium.UITest
{
    internal partial class XappiumTestEngine : ITestEngine
    {
        public const int SHORT_TIMEOUT = 5;
        public const int LONG_TIMEOUT = 30;

        private UITestConfiguration _config { get; }

        public XappiumTestEngine(UITestConfiguration config)
        {
            _config = config;
            SetupDriver();
        }

        public IReadOnlyDictionary<string, string> Settings => _config.Settings;

        public void Dispose()
            => StopApp();

        protected AppiumDriver<AppiumWebElement> Driver { get; private set; }

        protected WebDriverWait Wait { get; private set; }

        public Platform Platform => _config.Platform;

        void SetupDriver()
        {
            var options = new AppiumOptions();
            AddAdditionalCapability(options, "isHeadless", true);
            ConfigureCapabilities(options);

            var url = _config.AppiumServer;

            if (Platform == Platform.iOS)
            {
                // The value of DEVICE_NAME is only used for running on the iOS simulator,
                // but must also have some (any) value for iOS and Android physical devices
                AddAdditionalCapability(options, MobileCapabilityType.DeviceName, _config.DeviceName);
                if (!string.IsNullOrWhiteSpace(_config.AppPath))
                    AddAdditionalCapability(options, MobileCapabilityType.App, _config.AppPath);
                AddAdditionalCapability(options, MobileCapabilityType.PlatformName, "iOS");
                AddAdditionalCapability(options, MobileCapabilityType.AutomationName, "XCUITest");
                AddAdditionalCapability(options, "autoAcceptAlerts", true);

                Driver = new IOSDriver<AppiumWebElement>(url, options);
            }
            else if (Platform == Platform.Android)
            {
                if (!string.IsNullOrWhiteSpace(_config.AppPath))
                {
                    var apkFileInfo = new System.IO.FileInfo(_config.AppPath);
                    if (apkFileInfo.Exists)
                        options.AddAdditionalCapability(MobileCapabilityType.App, apkFileInfo.FullName);
                }

                AddAdditionalCapability(options, MobileCapabilityType.PlatformName, "Android");
                //options.AddAdditionalCapability(MobileCapabilityType.DeviceName, "Android Emulator");
                AddAdditionalCapability(options, MobileCapabilityType.DeviceName, _config.DeviceName);
                AddAdditionalCapability(options, MobileCapabilityType.Udid, _config.UDID);
                AddAdditionalCapability(options, "forceEspressoRebuild", true);
                AddAdditionalCapability(options, MobileCapabilityType.AutomationName, "Espresso");
                AddAdditionalCapability(options, "enforceAppInstall", true);

                Driver = new AndroidDriver<AppiumWebElement>(url, options);
            }
            else
            {
                throw new Exception("Appium Driver Setup not Implemented for this platform");
            }

            // Setup timeouts
            Driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(SHORT_TIMEOUT);
            Wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(LONG_TIMEOUT));
        }

        void AddAdditionalCapability(AppiumOptions options, string key, object value)
        {
            if (!_config.Capabilities.ContainsKey(key))
                options.AddAdditionalCapability(key, value);
        }

        void ConfigureCapabilities(AppiumOptions options)
        {
            foreach((string key, string value) in _config.Capabilities)
            {
                options.AddAdditionalCapability(key, value);
            }
        }

        public void StopApp()
        {
            if (Driver != null)
            {
                Driver.Quit();
                Driver = null;
                Wait = null;
                AppManager.AppStopped();
            }
        }

        public void Tap(string automationId)
            => Driver.FindElement(ByAutomationId(automationId))
                .Click();

        public void TapAtIndex(string automationId, int index)
            => Driver.FindElements(ByAutomationId(automationId))
                .ElementAt(index)
                .Click();

        public void TapWithText(string automationId, string text)
        {
            foreach (var elem in Driver.FindElements(ByAutomationId(automationId)))
            {
                if (elem.Text.Equals(text, StringComparison.OrdinalIgnoreCase))
                {
                    elem.Click();
                    return;
                }
            }
        }

        public void TapWithText(string text)
            => Driver.FindElementByXPath("//*[@text='" + text + "']").Click();

        public void TapWithClass(string automationId, string @class)
        {
            foreach (var elem in Driver.FindElements(ByAutomationId(automationId)))
            {
                if (elem.GetAttribute("className").Equals(@class, StringComparison.OrdinalIgnoreCase))
                {
                    elem.Click();
                    return;
                }
            }
        }

        public void TapClass(string @class)
            => Driver.FindElements(By.ClassName(@class))
                .FirstOrDefault()
                ?.Click();

        public void Screenshot(string title, [CallerMemberName] string methodName = null)
        {
            var st = new StackTrace();
            var i = 1;
            MethodBase method = null;
            do
            {
                method = st.GetFrame(i++).GetMethod();
                if (method.ReflectedType == typeof(BasePage))
                    method = null;
            } while (method is null);

            var className = method.ReflectedType.Name;
            var namespaceName = method.ReflectedType.Namespace;

            var baseDir = _config.ScreenshotsPath;

            var newFile = string.Format("{0}-{1}.jpg", methodName, title);
            var newDir = Path.Combine(baseDir, Driver.SessionId.ToString(), namespaceName, className);
            var fullPath = Path.Combine(newDir, newFile);
            Directory.CreateDirectory(newDir);

            var s = Driver.GetScreenshot();
            s.SaveAsFile(fullPath, ScreenshotImageFormat.Png);
        }

        public void DismissKeyboard()
            => Driver.HideKeyboard();

        public IUIElement EnterText(string automationId, string text)
        {
            var element = Driver.FindElement(ByAutomationId(automationId));
            element.SendKeys(text);
            return new UIElement(element);
        }

        public IUIElement WaitForElementWithText(string text)
        {
            var query = MobileBy.XPath("//*[@text='" + text + "']");
            Wait.Until(w => w.FindElement(query));
            return new UIElement(Driver.FindElement(query));
        }

        public void WaitForNoElementWithText(string text)
            => Wait.Until(w => w.FindElements(MobileBy.XPath("//*[@text='" + text + "']")).Count <= 0);

        public IUIElement WaitForElement(string automationId)
        {
            var query = ByAutomationId(automationId);
            Wait.Until(w => w.FindElement(query));
            return new UIElement(Driver.FindElement(query));
        }

        public void WaitForNoElement(string automationId)
            => Wait.Until(w => w.FindElements(ByAutomationId(automationId)).Count <= 0);

        public void AssertTextInElementByAutomationId(string automationId, string text)
        {
            var found = false;
            foreach (var elem in Driver.FindElements(ByAutomationId(automationId)))
            {
                if (elem.Text.IndexOf(text, StringComparison.InvariantCultureIgnoreCase) >= 0)
                {
                    found = true;
                    break;
                }
            }

            if (!found)
                throw new NullReferenceException(automationId + " did not contain the text '" + text + "'");
        }

        public void ScrollTo(string automationId)
        {
        }

        public IEnumerable<IUIElement> WaitForAnyElement(params string[] automationIds)
        {
            Wait.Until(w =>
                automationIds.Any(id => w.FindElements(ByAutomationId(id)).Any())
            );
            return automationIds.Select(x =>
                new UIElement(Driver.FindElement(ByAutomationId(x)))
            );
        }

        public bool ElementExists(string automationId)
            => Driver.FindElements(ByAutomationId(automationId)).Any();

        public void BackButton()
            => Driver.Navigate().Back();

        public void TouchAndHoldWithText(string text)
        {
            var e = Driver.FindElement(MobileBy.XPath("//*[@text='" + text + "']"));
            var touchAction = new TouchAction(Driver).LongPress(e);
            Driver.PerformTouchAction(touchAction);
        }

        public void SwipeToDelete(string automationId, int index)
        {
            var item = Driver.FindElement(ByAutomationId(automationId));
            var width = Driver.Manage().Window.Size.Width;

            var act = new TouchAction(Driver);
            var press = act.Press(item, width, item.Location.Y);
            var move = act.MoveTo(item, 0, item.Location.Y);
            var lift = act.Release();

            var action = new MultiAction(Driver);
            action.Add(press);
            action.Add(move);
            action.Add(lift);
            action.Perform();
        }

        string GetTagKeyViewMatcherJson(int resourceId, string automationId)
            => "{ \"name\" : \"withTagKey\", \"args\" : [ " + resourceId + ", { \"name\" : \"is\", \"args\" : \"" + automationId + "\" } ] }";

        public By ByAutomationId(string automationId) =>
            //Platform == TestPlatform.Android ? MobileBy.AndroidViewMatcher(GetTagKeyViewMatcherJson(8675309, automationId)) :
            MobileBy.AccessibilityId(automationId);
    }
}
