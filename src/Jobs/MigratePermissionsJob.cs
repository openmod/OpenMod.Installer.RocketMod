using OpenMod.API.Persistence;
using OpenMod.Core.Permissions.Data;
using OpenMod.Core.Persistence;
using Rocket.API.Serialisation;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using OpenMod.API.Users;
using OpenMod.Core.Helpers;
using OpenMod.Core.Users;
using Rocket.Core.Logging;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class MigratePermissionsJob : IReversibleJob
    {
        public void ExecuteMigration()
        {
            Logger.Log("Importing RocketMod permissions to OpenMod...");

            var rocketPermissionsPath = Path.Combine(ReadWrite.PATH, "Servers", Provider.serverID, "Rocket", "Permissions.config.xml");
            var deserializer = new XmlSerializer(typeof(RocketPermissions));

            using var stream = new FileStream(rocketPermissionsPath, FileMode.Open);
            var rocketPermissions = (RocketPermissions)deserializer.Deserialize(stream);

            var openmodRoles = new PermissionRolesData
            {
                Roles = new List<PermissionRoleData>()
            };

            var openmodUsers = new
            {
                Users = new List<UserData>()
            };

            // importing cooldowns is not possible as they are per command in OpenMod and per permissions in RocketMod
            foreach (var group in rocketPermissions.Groups)
            {
                openmodRoles.Roles.Add(new PermissionRoleData
                {
                    Id = group.Id,
                    Data = new Dictionary<string, object>
                    {
                        { "color", group.Color },
                        { "prefix", group.Prefix },
                        { "suffix", group.Suffix }
                    },
                    DisplayName = group.DisplayName,
                    IsAutoAssigned = group.Id.Equals(rocketPermissions.DefaultGroup, StringComparison.OrdinalIgnoreCase),
                    Parents = new HashSet<string> { group.ParentGroup ?? string.Empty }, // in rocket default id parameter ParentGroup is null
                    Permissions = new HashSet<string>(group.Permissions.Select(c => "RocketMod:" + c.Name)),
                    Priority = group.Priority
                });

                foreach (var member in group.Members)
                {
                    var userData = openmodUsers.Users.FirstOrDefault(d => d.Id == member);
                    if (userData == null)
                    {
                        userData = new UserData
                        {
                            Data = new Dictionary<string, object>(),
                            FirstSeen = DateTime.Now,
                            LastSeen = DateTime.Now,
                            Id = member,
                            LastDisplayName = member,
                            Permissions = new HashSet<string>(),
                            Roles = new HashSet<string>(),
                            Type = KnownActorTypes.Player
                        };

                        openmodUsers.Users.Add(userData);
                    }

                    userData.Roles.Add(group.Id);
                }
            }

            var datastore = new YamlDataStore(new DataStoreCreationParameters
            {
                ComponentId = "OpenMod.Core",
                Prefix = "openmod",
                Suffix = null,
                WorkingDirectory = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory
            });

            AsyncHelper.RunSync(async () => await datastore.SaveAsync("roles", openmodRoles));
            AsyncHelper.RunSync(async () => await datastore.SaveAsync("users", openmodUsers));
            Logger.Log($"Imported {openmodRoles.Roles.Count} permission group(s) and {openmodUsers.Users.Count} player(s) from RocketMod's Permission.config.xml to OpenMod.");
        }

        public void Revert()
        {
            Logger.Log("Deleting OpenMod permissions and roles files...");
          
            var workingDirectory = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory;
            var rolesFile  = Path.Combine(workingDirectory, "openmod.roles.yaml");
            if (File.Exists(rolesFile))
            {
                File.Delete(rolesFile);
            }

            var usersfile = Path.Combine(workingDirectory, "openmod.users.yaml");
            if (File.Exists(usersfile))
            {
                File.Delete(usersfile);
            }
        }
    }
}
