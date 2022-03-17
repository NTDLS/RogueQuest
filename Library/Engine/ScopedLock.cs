using System;
using System.Threading;

namespace Library.Engine
{
    public class ScopedLock
    {
        private ScopedKey _key;

        public ScopedKey Acquire()
        {
            lock (this)
            {
                while (_key != null)
                {
                    Thread.Sleep(1);
                }
                _key = new ScopedKey(this);
                return _key;
            }
        }

        public void TurnIn(ScopedKey key)
        {
            if (key == _key)
            {
                _key = null;
            }
            else
            {
                throw new Exception("Invalid ScopedKey.");
            }
        }
    }
}
