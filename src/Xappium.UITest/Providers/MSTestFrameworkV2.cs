using System;
using System.Reflection;

namespace Xappium.UITest.Providers
{
    internal class MSTestFrameworkV2 : LateBoundTestFramework
    {
        protected override string ExceptionFullName => "Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException";

        protected internal override string AssemblyName => "Microsoft.VisualStudio.TestPlatform.TestFramework";

        public override void WriteLine(string message)
        {
            var loggerType = Type.GetType("Microsoft.VisualStudio.TestTools.UnitTesting.Logging.Logger");
            var logMessageMethod = loggerType.GetMethod("LogMessage", BindingFlags.Public | BindingFlags.Static);
            logMessageMethod.Invoke(null, new object[] { message, Array.Empty<object>() });
        }
    }
}
