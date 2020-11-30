using OpenMod.API.Persistence;
using OpenMod.Core.Permissions.Data;
using OpenMod.Core.Persistence;
using OpenMod.Installer.RocketMod.Helpers;
using Rocket.API.Serialisation;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;

namespace OpenMod.Installer.RocketMod.Jobs
{
    [Prevent("--no-migration")]
    [Prevent("--no-permission-link")]
    public class MigratePermissionsJob : IJob, IRevertable
    {
        public void ExecuteMigration()
        {
            var rocketPermissionsPath = Path.Combine(ReadWrite.PATH, "Servers", Provider.serverID, "Rocket", "Permissions.config.xml");
            var deserializer = new XmlSerializer(typeof(RocketPermissions));

            using var stream = new FileStream(rocketPermissionsPath, FileMode.Open);
            var rocketPermissions = (RocketPermissions)deserializer.Deserialize(stream);

            var openmodRoles = new PermissionRolesData
            {
                Roles = new List<PermissionRoleData>()
            };

            foreach (var permission in rocketPermissions.Groups)
            {
                openmodRoles.Roles.Add(new PermissionRoleData
                {
                    Id = permission.Id,
                    Data = new Dictionary<string, object>(),
                    DisplayName = permission.DisplayName,
                    IsAutoAssigned = permission.Id.Equals(rocketPermissions.DefaultGroup, StringComparison.OrdinalIgnoreCase),
                    Parents = new HashSet<string> { permission.ParentGroup },
                    Permissions = new HashSet<string>(permission.Permissions.Select(c => "Rocket.PermissionLink:" + c.Name)),
                    Priority = permission.Priority
                });
            }

            var datastore = new YamlDataStore(new DataStoreCreationParameters
            {
                ComponentId = "OpenMod.Core",
                Prefix = "openmod",
                Suffix = null,
                WorkingDirectory = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory
            });

            AsyncHelper.RunSync(() => datastore.SaveAsync("roles", openmodRoles));
        }

        public void Revert()
        {
            var path = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory + "/openmod.roles.yaml";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}
