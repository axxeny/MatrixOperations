using System;
using System.Threading;
using System.Threading.Tasks;

namespace AxxenyUtilities.Helpers
{
    public static class SemaphoreSlimExtensions
    {
        public static void Lock(this SemaphoreSlim semaphore, TimeSpan timeSpan, Action action)
        {
            if (!semaphore.Wait(timeSpan))
            {
                return;
            }
            try
            {
                action();
            }
            catch (Exception)
            {
                semaphore.Release();
                throw;
            }
            semaphore.Release();
        }

        public static async Task LockAsync(this SemaphoreSlim semaphore, Func<Task> action)
        {
            await semaphore.WaitAsync();
            try
            {
                await action();
            }
            catch(Exception)
            {
                semaphore.Release();
                throw;
            }
            semaphore.Release();
        }
    }
}
