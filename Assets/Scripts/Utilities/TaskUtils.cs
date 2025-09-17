using System;
using System.Threading.Tasks;

namespace Utilities
{
    public static class TaskUtils
    {
        public static async Task<T> Then<T>(this Task<T> task, Action<T> action)
        {
            var result = await task;
            action(result);
            return result;
        }

        public static async Task Then(this Task task, Action action)
        {
            await task;
            action();
        }

        public static void Forget(this Task task)
        {
            task.ContinueWith(t => t.Exception, TaskContinuationOptions.OnlyOnFaulted);
        }
    }
}