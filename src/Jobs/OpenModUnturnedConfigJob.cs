using OpenMod.Installer.RocketMod.Helpers;
using Rocket.Core.Logging;
using System.IO;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public abstract class OpenModUnturnedConfigJob : IJob
    {
        public void ExecuteMigration(string[] args)
        {
            Logger.Log($"Updating OpenMod Unturned {ConfigType} config...");

            var openmodDirectory = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory;
            var openmodUnturnedYaml = Path.Combine(openmodDirectory, "openmod.unturned.yaml");

            if (!Directory.Exists(openmodDirectory))
            {
                Directory.CreateDirectory(openmodDirectory);
            }

            string configText;

            if (File.Exists(openmodUnturnedYaml))
            {
                configText = File.ReadAllText(openmodUnturnedYaml);
            }
            else
            {
                var openmodAssembly = AssemblyHelper.GetAssembly("OpenMod.Unturned");
                using var configFileStream = openmodAssembly.GetManifestResourceStream("OpenMod.Unturned.openmod.unturned.yaml");
                using var ms = new MemoryStream();
                configFileStream!.CopyTo(ms);

                ms.Seek(0, SeekOrigin.Begin);

                using var reader = new StreamReader(ms);
                configText = reader.ReadToEnd();
            }

            ModifyConfig(ref configText);

            File.WriteAllText(openmodUnturnedYaml, configText);

            Logger.Log($"Finished updating OpenMod Unturned {ConfigType} config.");
        }

        protected abstract string ConfigType { get; }

        protected abstract void ModifyConfig(ref string configText);
    }
}
