namespace Xappium.UITest.Providers
{
    internal class FallbackTestFramework : ITestFramework
    {
        /// <summary>
        /// Gets a value indicating whether the corresponding test framework is currently available.
        /// </summary>
        public bool IsAvailable => true;

        /// <summary>
        /// Throws a framework-specific exception to indicate a failing unit test.
        /// </summary>
        public void Throw(string message)
        {
            throw new AssertionFailedException(message);
        }

        public virtual void AttachFile(string filePath, string description)
        {
        }
    }
}
