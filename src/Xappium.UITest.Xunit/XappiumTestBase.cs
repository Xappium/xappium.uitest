using System;
using Xunit;

namespace Xappium.UITest
{
    [Collection(nameof(XappiumTest))]
    public class XappiumTestBase : IDisposable
    {
        protected ITestEngine Engine { get; }

        public XappiumTestBase()
        {
            AppManager.StartApp();
            Engine = AppManager.Engine;
        }

        ~XappiumTestBase ()
        {
            Dispose(false);
        }

        public virtual void Dispose()
        {
            Dispose(true);
        }

        bool disposed;
        void Dispose (bool disposing)
        {
            if (!disposed)
            {
                if (disposing)
                    DisposeManagedResources();
                DisposeUnmanagedResources();
                disposed = false;
            }
        }

        protected virtual void DisposeManagedResources ()
        {
            Engine.StopApp();
        }

        protected virtual void DisposeUnmanagedResources ()
        {
        }
    }
}
