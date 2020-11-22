namespace OpenMod.Installer.RocketMod.Migrations
{
    public interface IJob
    {
        string Id { get; }

        void ExecuteMigration();
    }
}