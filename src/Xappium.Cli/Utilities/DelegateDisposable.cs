using System;

namespace Xappium.Utilities
{
    internal class DelegateDisposable : IDisposable
    {
        private WeakReference<Action> _weakDelegate;

        public DelegateDisposable(Action @delegate)
        {
            _weakDelegate = new WeakReference<Action>(@delegate);
        }

        public void Dispose()
        {
            if(_weakDelegate != null && _weakDelegate.TryGetTarget(out var target))
            {
                target?.Invoke();
                _weakDelegate = null;
            }
        }
    }
}
