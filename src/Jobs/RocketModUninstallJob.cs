using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class RocketModUninstallJob : IJob, IRevertable
    {
        public void ExecuteMigration()
        {
            var rocketModulePath = Path.Combine(ReadWrite.PATH, "Modules", "Rocket.Unturned", "Rocket.Unturned.module");
            var renamedRocketModulePath = rocketModulePath + ".bak";
            if (File.Exists(rocketModulePath))
            {
                File.Move(rocketModulePath, renamedRocketModulePath);
            }
        }

        public void Revert()
        {
            var rocketModulePath = Path.Combine(ReadWrite.PATH, "Modules", "Rocket.Unturned", "Rocket.Unturned.module");
            var renamedRocketModulePath = rocketModulePath + ".bak";
            if (File.Exists(renamedRocketModulePath))
            {
                File.Move(renamedRocketModulePath, rocketModulePath);
            }
        }
    }
}
