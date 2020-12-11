using System.IO;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class FileCreateJob : IJob, IRevertable
    {
        public void ExecuteMigration()
        {
            File.WriteAllText(Path.GetTempPath() + "/openmod-unturned", string.Empty);
        }

        public void Revert()
        {
            var path = Path.GetTempPath() + "/openmod-unturned";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
