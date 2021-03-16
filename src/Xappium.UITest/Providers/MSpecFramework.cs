namespace Xappium.UITest.Providers
{
    internal class MSpecFramework : LateBoundTestFramework
    {
        protected internal override string AssemblyName => "Machine.Specifications";

        protected override string ExceptionFullName => "Machine.Specifications.SpecificationException";
    }
}
