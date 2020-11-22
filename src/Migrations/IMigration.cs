namespace OpenMod.Installer.RocketMod.Migrations
{
    public interface IMigration
    {
        string Id { get; }

        void ExecuteMigration();
    }
}