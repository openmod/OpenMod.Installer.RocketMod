using System;
using System.IO;
using System.Linq;
using OpenMod.Installer.RocketMod.Jobs;
using Rocket.Core.Plugins;
using SDG.Unturned;
using UnityEngine;

// todo: main command logs
// NuGetInstallJob implementation by using OpenMod.NuGet
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

            var openmodPath = Path.Combine(ReadWrite.PATH, "Modules", "OpenMod.Unturned");
            OpenModManager = new OpenModManager(openmodPath);
            JobsManager = new JobsManager();

            JobsManager.RegisterJob(new OpenModModuleInstallJob());
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
