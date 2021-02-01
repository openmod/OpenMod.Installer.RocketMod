using System;
using System.Threading.Tasks;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class AsyncHelperEx
    {
        public static void RunSync(Func<Task> task)
        {
            task().GetAwaiter().GetResult();
        }
    }
}