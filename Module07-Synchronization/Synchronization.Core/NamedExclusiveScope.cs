using System;
using System.Threading;

namespace Synchronization.Core
{
    /*
     * Implement very simple wrapper around Mutex or Semaphore (remember both implement WaitHandle) to
     * provide a exclusive region created by `using` clause.
     *
     * Created region may be system-wide or not, depending on the constructor parameter.
     */
    public sealed class NamedExclusiveScope : IDisposable
    {
        private readonly Mutex _mutex;
        public NamedExclusiveScope(string name, bool isSystemWide)
        {
            var mutexName = $"{(isSystemWide ? "Global" : "Local")}\\{name}"; 
            _mutex = new Mutex(false, mutexName, out var createdNew);
            if (!createdNew)
            {
                throw new InvalidOperationException($"Unable to get a global lock {name}.");
            }
            
            _mutex.WaitOne();
        }

        public void Dispose()
        {
            _mutex.ReleaseMutex();
            _mutex?.Dispose();
        }
    }
}
