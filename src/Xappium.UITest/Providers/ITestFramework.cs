namespace Xappium.UITest.Providers
{
    internal interface ITestFramework
    {
        bool IsAvailable { get; }

        void Throw(string message);

        void AttachFile(string filePath, string description);
    }
}
