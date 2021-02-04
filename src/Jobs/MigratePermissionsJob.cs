using OpenMod.Installer.RocketMod.Helpers;
using Rocket.API.Serialisation;
using Rocket.Core.Logging;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class MigratePermissionsJob : IReversibleJob
    {
        public void ExecuteMigration(string[] args)
        {
            Logger.Log("Importing RocketMod permissions to OpenMod...");

            var rocketPermissionsPath = Path.Combine(ReadWrite.PATH, "Servers", Provider.serverID, "Rocket", "Permissions.config.xml");
            var deserializer = new XmlSerializer(typeof(RocketPermissions));

            using var stream = new FileStream(rocketPermissionsPath, FileMode.Open);
            var rocketPermissions = (RocketPermissions)deserializer.Deserialize(stream);

            var openmodRoles = new
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
                    var userData = openmodUsers.Users.Find(d => d.Id == member);
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
                            Type = "player"
                        };

                        openmodUsers.Users.Add(userData);
                    }

                    userData.Roles.Add(group.Id);
                }
            }

            var apiAssembly = AssemblyHelper.GetAssembly("OpenMod.API");
            var datastoreCreationParametersType = apiAssembly.GetType("OpenMod.API.Persistence.DataStoreCreationParameters");
            var workingDirectory = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory;

            var @params = Activator.CreateInstance(datastoreCreationParametersType);
            @params.SetPropertyValue("ComponentId", "OpenMod.Core");
            @params.SetPropertyValue("Prefix", "openmod");
            @params.SetPropertyValue("WorkingDirectory", workingDirectory);

            var coreAssembly = AssemblyHelper.GetAssembly("OpenMod.Core");
            var datastoreType = coreAssembly.GetType("OpenMod.Core.Persistence.YamlDataStore");
            var ctor = datastoreType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { datastoreCreationParametersType }, null);

            var dataStore = ctor.Invoke(new[] { @params });
            var saveMethod = dataStore.GetType().GetMethod("SaveAsync", BindingFlags.Instance | BindingFlags.Public);
            var saveMethodTaskRoles = (Task)saveMethod.Invoke(dataStore, new object[] { "roles", openmodRoles });
            var saveMethodTaskUsers = (Task)saveMethod.Invoke(dataStore, new object[] { "users", openmodUsers });

            AsyncHelperEx.RunSync(() => saveMethodTaskRoles);
            AsyncHelperEx.RunSync(() => saveMethodTaskUsers);
            Logger.Log($"Imported {openmodRoles.Roles.Count} permission group(s) and {openmodUsers.Users.Count} player(s) from RocketMod's Permission.config.xml to OpenMod.");
        }

        public void Revert()
        {
            Logger.Log("Deleting OpenMod users and roles files...");

            var workingDirectory = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory;
            var rolesFile = Path.Combine(workingDirectory, "openmod.roles.yaml");
            if (File.Exists(rolesFile))
            {
                File.Delete(rolesFile);
            }

            var usersfile = Path.Combine(workingDirectory, "openmod.users.yaml");
            if (File.Exists(usersfile))
            {
                File.Delete(usersfile);
            }

            Logger.Log("Deleted OpenMod users and role files.");
        }
    }

    public class UserData
    {
        public string Id { get; set; }
        public string Type { get; set; }

        public string LastDisplayName { get; set; }

        public DateTime FirstSeen { get; set; }

        public DateTime LastSeen { get; set; }

        public HashSet<string> Permissions { get; set; }

        public HashSet<string> Roles { get; set; }
        public Dictionary<string, object> Data { get; set; }
    }

    public class PermissionRoleData
    {
        public string Id { get; set; }
        public int Priority { get; set; }
        public HashSet<string> Parents { get; set; }
        public HashSet<string> Permissions { get; set; }
        public string DisplayName { get; set; }
        public Dictionary<string, object> Data { get; set; }
        public bool IsAutoAssigned { get; set; }
    }
}
