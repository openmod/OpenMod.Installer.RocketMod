using System;
using OpenMod.Installer.RocketMod.Jobs;
using Rocket.Core.Plugins;

namespace OpenMod.Installer.RocketMod
{
    public class OpenModInstallerPlugin : RocketPlugin
    {
        public static OpenModInstallerPlugin Instance { get; private set; }
        public OpenModManager OpenModManager { get; private set; }
        public JobsManager JobsManager { get; private set; }

        protected override void Load()
        {
            Instance = this;
            
            OpenModManager = new OpenModManager("todo: find or create openmod directory");
            JobsManager = new JobsManager();

            // JobsManager.RegisterJob(new OpenModInstallJob());

            base.Load();
        }

        protected override void Unload()
        {
            base.Unload();
            Instance = null;
        }
    }
}
