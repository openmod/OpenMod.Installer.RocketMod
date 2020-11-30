using OpenMod.Installer.RocketMod.Jobs;
using OpenMod.Installer.RocketMod.Jobs.OpenModPackagesInstallJobs;
using Rocket.Core.Plugins;
using SDG.Unturned;
using System;
using System.IO;
using System.Linq;

// todo: main command logs
// OpenModRocketModUninstallJob (rename RocketMod module to module.bak and rename OpenMod to .bak)
// OpenModUconomyMigrationJob (execute /migrate of OpenMod.Economy after next restart)

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

            var openmodPath = Path.Combine(ReadWrite.PATH, "Modules", "OpenMod.Unturned");
            var packagesPath = Path.Combine(ReadWrite.PATH, "Servers", Provider.serverID, "OpenMod", "packages");
            OpenModManager = new OpenModManager(openmodPath, packagesPath);
            JobsManager = new JobsManager();

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

            base.Load();
        }

        protected override void Unload()
        {
            base.Unload();
            Instance = null;
        }
    }
}
