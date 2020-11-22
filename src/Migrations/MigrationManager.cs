using System.Collections.Generic;

namespace OpenMod.Installer.RocketMod.Migrations
{
    public class MigrationManager
    {
        private readonly HashSet<IMigration> _migrations;

        public MigrationManager()
        {
            _migrations = new HashSet<IMigration>();
        }

        public void RegisterMigration(IMigration migration)
        {
            _migrations.Add(migration);
        }

        public IReadOnlyCollection<IMigration> GetMigrations()
        {
            return _migrations;
        }
    }
}