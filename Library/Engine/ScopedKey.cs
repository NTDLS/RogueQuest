using System;

namespace Library.Engine
{
    public class ScopedKey : IDisposable
    {
        private bool _disposed = false;

        private ScopedLock _parent;

        public ScopedKey(ScopedLock parent)
        {
            _parent = parent;
        }

        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                // Dispose managed state (managed objects).
            }

            _parent.TurnIn(this);

            _disposed = true;
        }
    }
}
