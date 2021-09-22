using System;
using System.Linq;

namespace Xappium.UITest.Providers
{
    internal class NUnitTestFramework : LateBoundTestFramework
    {
        protected internal override string AssemblyName => "nunit.framework";

        protected override string ExceptionFullName => "NUnit.Framework.AssertionException";

        public override void AttachFile(string filePath, string description)
        {
            try
            {
                var type = Type.GetType("NUnit.Framework.TestContext");
                var addTestAttachment = type.GetMethods(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.Public)
                    .FirstOrDefault(x => x.Name == "AddTestAttachment" && x.GetParameters().Length == 2);
                addTestAttachment.Invoke(null, new[] { filePath, description });
            }
            catch
            {
                // Suppress errors
            }
        }
    }
}
