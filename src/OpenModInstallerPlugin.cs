using Rocket.Core.Plugins;

namespace OpenMod.Installer.RocketMod
{
    public class OpenModInstallerPlugin : RocketPlugin
    {
        public static OpenModInstallerPlugin Instance { get; private set; }
        public OpenModManager OpenModManager { get; private set; }

        protected override void Load()
        {
            Instance = this;

            OpenModManager = new OpenModManager();
            base.Load();
        }

        protected override void Unload()
        {
            base.Unload();
            Instance = null;
        }
    }
}
