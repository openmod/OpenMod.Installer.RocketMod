namespace OpenMod.Installer.RocketMod.Jobs
{
    public interface IJob
    {
        void ExecuteMigration(string[] args);
    }
}