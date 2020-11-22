using System;
using Rocket.Core.Plugins;

namespace OpenMod.Installer.RocketMod
{
    public class OpenModInstallerPlugin : RocketPlugin
    {
        public static OpenModInstallerPlugin Instance { get; private set; }

        protected override void Load()
        {
            Instance = this;
            base.Load();
        }

        protected override void Unload()
        {
            base.Unload();
            Instance = null;
        }
    }
}
