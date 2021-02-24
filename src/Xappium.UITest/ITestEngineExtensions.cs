using System.Runtime.CompilerServices;

namespace Xappium.UITest
{
    public static class ITestEngineExtensions
    {
        public static void Screenshot(this ITestEngine engine, [CallerMemberName] string methodName = null) =>
            engine.Screenshot(string.Empty, methodName);

        public static void Tap(this ITestEngine engine, string automationId) =>
            engine.Tap(automationId, null);

        public static IUIElement EnterText(this ITestEngine engine, string automationId, string text) =>
            engine.EnterText(automationId, text, null);

        public static IUIElement WaitForElementWithText(this ITestEngine engine, string text) =>
            engine.WaitForElementWithText(text, null);

        public static void WaitForNoElementWithText(this ITestEngine engine, string text) =>
            engine.WaitForNoElementWithText(text, null);

        public static IUIElement WaitForElement(this ITestEngine engine, string automationId) =>
            engine.WaitForElement(automationId, null);

        public static void WaitForNoElement(this ITestEngine engine, string automationId) =>
            engine.WaitForNoElement(automationId, null);

        public static void AssertTextInElementByAutomationId(this ITestEngine engine, string automationId, string text) =>
            engine.AssertTextInElementByAutomationId(automationId, text, null);

        public static bool ElementExists(this ITestEngine engine, string automationId) =>
            engine.ElementExists(automationId, null);
    }
}
