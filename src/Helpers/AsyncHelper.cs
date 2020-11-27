using Nito.AsyncEx;
using System;
using System.Threading.Tasks;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class AsyncHelper
    {
        public static void RunSync(Func<Task> func)
        {
            AsyncContext.Run(func);
        }
    }
}
