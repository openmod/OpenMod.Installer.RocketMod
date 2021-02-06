using Nito.AsyncEx;
using System;
using System.Threading.Tasks;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class AsyncHelperEx
    {
        public static void RunSync(Func<Task> task)
        {
            AsyncContext.Run(task);
        }

        public static T RunSync<T>(Func<Task<T>> task)
        {
            return AsyncContext.Run(task);
        }
    }
}