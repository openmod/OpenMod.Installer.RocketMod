using SDG.Unturned;
using System.IO;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class RocketModInstallJob : IJob, IRevertable
    {
        public void ExecuteMigration()
        {
            var rocketModulePath = Path.Combine(ReadWrite.PATH, "Modules", "Rocket.Unturned", "Rocket.Unturned.module");
            var renamedRocketModulePath = rocketModulePath + ".bak";
            if (File.Exists(renamedRocketModulePath))
            {
                File.Move(renamedRocketModulePath, rocketModulePath);
            }
        }

        public void Revert()
        {
            var rocketModulePath = Path.Combine(ReadWrite.PATH, "Modules", "Rocket.Unturned", "Rocket.Unturned.module");
            var renamedRocketModulePath = rocketModulePath + ".bak";
            if (File.Exists(rocketModulePath))
            {
                File.Move(rocketModulePath, renamedRocketModulePath);
            }
        }
    }
}
