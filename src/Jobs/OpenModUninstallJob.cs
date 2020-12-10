using System.IO;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class OpenModUninstallJob : IJob, IRevertable
    {
        public void ExecuteMigration()
        {
            var openmodModulePath = OpenModInstallerPlugin.Instance.OpenModManager.ModuleDirectory + "/OpenMod.Unturned.module";
            var renamedOpenmodModulePath = openmodModulePath + ".bak";
            if (File.Exists(openmodModulePath))
            {
                File.Move(openmodModulePath, renamedOpenmodModulePath);
            }
        }

        public void Revert()
        {
            var openmodModulePath = OpenModInstallerPlugin.Instance.OpenModManager.ModuleDirectory + "/OpenMod.Unturned.module";
            var renamedOpenmodModulePath = openmodModulePath + ".bak";
            if (File.Exists(renamedOpenmodModulePath))
            {
                File.Move(renamedOpenmodModulePath, openmodModulePath);
            }
        }
    }
}
