namespace OpenMod.Installer.RocketMod
{
    public class OpenModManager
    {
        public OpenModManager(string workingDirectory, string packagesDirectory)
        {
            WorkingDirectory = workingDirectory;
            PackagesDirectory = packagesDirectory;
        }

        public string WorkingDirectory { get; } // gets the openmod directory
        public string PackagesDirectory { get; }
    }
}