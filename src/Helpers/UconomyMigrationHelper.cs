using Autofac;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.Core.Console;
using System.IO;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class UconomyMigrationHelper
    {
        public static void Migrate()
        {
            var module = OpenModUnturnedModuleHelper.GetOpenModModule();
            var runtime = (IRuntime)module.OpenModRuntime;
            var logger = runtime.LifetimeScope.Resolve<ILoggerFactory>().CreateLogger<ConsoleActor>();

            AsyncHelper.RunSync(() => runtime.LifetimeScope.Resolve<ICommandExecutor>()
                .ExecuteAsync(new ConsoleActor(logger, "openmod-unturned"),
                    new string[] { "migrateuconomy" }, null));

            File.Delete(Path.GetTempPath() + "/openmod-unturned");
        }
    }
}
