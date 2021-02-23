using System.Runtime.CompilerServices;

namespace Xappium.UITest
{
    public static class ITestEngineExtensions
    {
        public static void Screenshot(this ITestEngine engine, [CallerMemberName] string methodName = null) =>
            engine.Screenshot(string.Empty, methodName);
    }
}
