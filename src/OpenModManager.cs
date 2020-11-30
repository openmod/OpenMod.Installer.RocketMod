namespace OpenMod.Installer.RocketMod
{
    public class OpenModManager
    {
        public OpenModManager(string workingDirectory, string moduleDirectory)
        {
            WorkingDirectory = workingDirectory;
            ModuleDirectory = moduleDirectory;
        }

        public string WorkingDirectory { get; } // gets the openmod directory
        public string ModuleDirectory { get; } // gets the openmod module directory
    }
}