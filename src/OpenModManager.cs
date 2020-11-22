namespace OpenMod.Installer.RocketMod
{
    public class OpenModManager
    {
        public OpenModManager(string workingDirectory)
        {
            WorkingDirectory = workingDirectory;
        }

        public string WorkingDirectory { get; } // gets the openmod directory
    }
}