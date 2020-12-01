using OpenMod.API.Persistence;
using OpenMod.Core.Permissions.Data;
using OpenMod.Core.Persistence;
using OpenMod.Installer.RocketMod.Helpers;
using OpenMod.Installer.RocketMod.Models;
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

            foreach (var group in rocketPermissions.Groups)
            {
                openmodRoles.Roles.Add(new PermissionRoleData
                {
                    Id = group.Id,
                    Data = new Dictionary<string, object>
                    {
                        { "color", group.Color },
                        { "prefix", group.Prefix },
                        { "suffix", group.Suffix },
                        { "cooldowns", GetCooldowns(group.Permissions) }
                    },
                    DisplayName = group.DisplayName,
                    IsAutoAssigned = group.Id.Equals(rocketPermissions.DefaultGroup, StringComparison.OrdinalIgnoreCase),
                    Parents = new HashSet<string> { group.ParentGroup ?? string.Empty }, // in rocket default id parameter ParentGroup is null
                    Permissions = new HashSet<string>(group.Permissions.Select(c => "Rocket.PermissionLink:" + c.Name)),
                    Priority = group.Priority
                });
            }

            var datastore = new YamlDataStore(new DataStoreCreationParameters
            {
                ComponentId = null, // ComponentId is never used in YamlDataStore lol
                Prefix = "openmod",
                Suffix = null,
                WorkingDirectory = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory
            });

            AsyncHelper.RunSync(async () => await datastore.SaveAsync("roles", openmodRoles));
        }

        public void Revert()
        {
            var path = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory + "/openmod.roles.yaml";
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }

        private HashSet<CooldownSpan> GetCooldowns(List<Permission> permissions)
        {
            var cooldowns = new HashSet<CooldownSpan>();
            foreach (var permission in permissions)
            {
                cooldowns.Add(new CooldownSpan
                {
                    Command = "rocket." + permission.Name,
                    Cooldown = $"{permission.Cooldown} seconds"
                });
            }
            return cooldowns;
        }
    }
}
