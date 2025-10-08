using System;
using Cysharp.Threading.Tasks;

namespace Utilities
{
    public static class TaskUtils
    {
        public static async UniTask<T> Then<T>(this UniTask<T> task, Action<T> action)
        {
            var result = await task;
            action(result);
            return result;
        }

        public static async UniTask Then(this UniTask task, Action action)
        {
            await task;
            action();
        }

        // UniTask already has Forget() method built-in, but keeping this for compatibility
        public static void Forget(this UniTask task)
        {
            task.Forget();
        }
    }
}