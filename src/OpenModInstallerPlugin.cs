using System;
using System.Linq;
using OpenMod.Installer.RocketMod.Jobs;
using Rocket.Core.Plugins;

// todo: main command logs
// Setting OpenMod directory in OpenModManager.WorkingDirectory (see below in load); required for some jobs so they can get the OpenMod folder
// NuGetInstallJob implementation by using OpenMod.NuGet
// OpenModInstallJob (download & install OpenMod module; rename RocketMod module to .bak) (Sqidrod)
// OpenModRocketModUninstallJob (rename RocketMod module to module.bak and rename OpenMod to .bak)
// OpenModPermissionsMigrationJob (migrate permissions, only if PermissionLink will get installed)
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
            
            OpenModManager = new OpenModManager("todo: find or create openmod directory");
            JobsManager = new JobsManager();

            JobsManager.RegisterJob(new OpenModCooldownsInstallJob());
            JobsManager.RegisterJob(new OpenModEconomyInstallJob());
            JobsManager.RegisterJob(new OpenModPermissionLinkInstallJob());
            JobsManager.RegisterJob(new OpenModPermissionsExInstallJob());
            JobsManager.RegisterJob(new OpenModRocketModBridgeInstallJob());
            JobsManager.RegisterJob(new OpenModUconomyToOpenModInstallJob());

            base.Load();
        }

        protected override void Unload()
        {
            base.Unload();
            Instance = null;
        }
    }
}
