using Autofac;
using Microsoft.Extensions.Logging;
using OpenMod.API;
using OpenMod.API.Commands;
using OpenMod.Core.Console;
using OpenMod.Installer.RocketMod.Helpers;
using OpenMod.Installer.RocketMod.Jobs;
using OpenMod.Installer.RocketMod.Jobs.OpenModPackagesInstallJobs;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using System.IO;
using System.Linq;

// todo: main command logs

namespace OpenMod.Installer.RocketMod
{
    public class OpenModInstallerPlugin : RocketPlugin
    {
        public static OpenModInstallerPlugin Instance { get; private set; }
        public OpenModManager OpenModManager { get; private set; }
        public JobsManager JobsManager { get; private set; }
        public bool IsOpenModRocketModBridge { get; private set; }

        protected override void Load()
        {
            IsOpenModRocketModBridge = AppDomain.CurrentDomain.GetAssemblies().Any(d => d.FullName.Contains("OpenMod.Core"));

            Instance = this;

            var openmodModulePath = Path.Combine(ReadWrite.PATH, "Modules", "OpenMod.Unturned");
            var openmodWorkingPath = Path.Combine(ReadWrite.PATH, "Servers", Provider.serverID, "OpenMod");
            OpenModManager = new OpenModManager(openmodWorkingPath, openmodModulePath);
            JobsManager = new JobsManager();

            if (!IsOpenModRocketModBridge)
            {
                JobsManager.RegisterJob(new OpenModModuleInstallJob());
                JobsManager.RegisterJob(new RocketModUninstallJob());
                JobsManager.RegisterJob(new AssemblyLoadJob());
                JobsManager.RegisterJob(new OpenModUnturnedInstallJob());
                JobsManager.RegisterJob(new OpenModUnityEngineInstallJob());
                JobsManager.RegisterJob(new OpenModCooldownsInstallJob());
                JobsManager.RegisterJob(new OpenModEconomyInstallJob());
                JobsManager.RegisterJob(new OpenModPermissionLinkInstallJob());
                JobsManager.RegisterJob(new OpenModPermissionsExInstallJob());
                JobsManager.RegisterJob(new OpenModRocketModBridgeInstallJob());
                JobsManager.RegisterJob(new OpenModUconomyToOpenModInstallJob());
                JobsManager.RegisterJob(new MigratePermissionsJob());
            }
            else
            {
                JobsManager.RegisterJob(new OpenModUninstallJob());
                JobsManager.RegisterJob(new RocketModInstallJob());
            }

            if (IsOpenModRocketModBridge)
            {
                var module = OpenModUnturnedModuleHelper.GetOpenModModule();
                var runtime = (IRuntime)module.OpenModRuntime;
                var logger = runtime.LifetimeScope.Resolve<ILoggerFactory>().CreateLogger<ConsoleActor>();

                AsyncHelper.RunSync(() => runtime.LifetimeScope.Resolve<ICommandExecutor>()
                    .ExecuteAsync(new ConsoleActor(logger, "openmod-unturned"),
                        new string[] { "migrateuconomy" }, null));
            }

            base.Load();
        }

        protected override void Unload()
        {
            base.Unload();
            Instance = null;
        }
    }
}
