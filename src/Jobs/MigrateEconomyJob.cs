using System.Collections.Generic;
using System.IO;
using System.Linq;
using OpenMod.API.Jobs;
using OpenMod.API.Persistence;
using OpenMod.Core.Helpers;
using OpenMod.Core.Jobs;
using OpenMod.Core.Persistence;
using OpenMod.Installer.RocketMod.Helpers;
using Rocket.Core.Logging;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class MigrateEconomyJob : IJob
    {
        public void ExecuteMigration(string[] args)
        {
            Logger.Log("Adding economy migration job to autoexec.yaml...");
            var openmodDirectory = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory;
            var autoexecYaml = Path.Combine(openmodDirectory, "autoexec.yaml");

            var dataStore = new YamlDataStore(new DataStoreCreationParameters
            {
                WorkingDirectory = openmodDirectory,
                LogOnChange = false
            }, null, null, null);

            if (!AsyncHelper.RunSync(() => dataStore.ExistsAsync("autoexec")))
            {
                var openmodAssembly = typeof(YamlDataStore).Assembly;
                using var autoexecFileStream = openmodAssembly.GetManifestResourceStream("OpenMod.Core.autoexec.yaml");
                using var ms = new MemoryStream();
                autoexecFileStream!.CopyTo(ms);

                ms.Seek(0, SeekOrigin.Begin);
                File.WriteAllBytes(autoexecYaml, ms.ToArray());
            }

            var jobsFile = AsyncHelperEx.RunSync(() => dataStore.LoadAsync<ScheduledJobsFile>("autoexec"));
            if (jobsFile!.Jobs!.Any(d => d.Name?.Equals("OpenMod.Installer.EconomyMigration") ?? false))
            {
                return;
            }

            jobsFile.Jobs.Add(new ScheduledJob
            {
                Name = "OpenMod.Installer.EconomyMigration",
                Enabled = true,
                Task = "openmod_command",
                Args = new Dictionary<string, object>
                {
                    { "commands", new List<string>
                        {
                            "MigrateUconomy"
                        }
                    }
                },
                Schedule = "@single_exec"
            });

            AsyncHelperEx.RunSync(() => dataStore.SaveAsync("autoexec", jobsFile));
        }
    }
}