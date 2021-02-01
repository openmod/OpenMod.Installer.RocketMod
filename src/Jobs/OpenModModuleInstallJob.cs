using System.Collections.Generic;
using Newtonsoft.Json;
using Rocket.Core.Logging;
using System.IO;
using System.Net;
using OpenMod.Installer.RocketMod.Helpers;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class OpenModModuleInstallJob : IReversibleJob
    {
        private bool m_OpenModInstalledAlready;

        public void ExecuteMigration()
        {
            Logger.Log("Downloading and installing the OpenMod module.");

            if (OpenModInstallerPlugin.Instance.OpenModManager.IsOpenModInstalled)
            {
                m_OpenModInstalledAlready = true;
            }

            using var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "request");

            // todo: query *all* releases, not only the latest
            var releaseData = webClient.DownloadString("https://api.github.com/repos/openmod/openmod/releases/latest");
            var release = JsonConvert.DeserializeObject<GitHubRelease>(releaseData);

            //this can never be empty UNLESS OpenMod posts a release without the Unturned Module
            var moduleAsset = release.Assets.Find(x => x.BrowserDownloadUrl?.Contains("OpenMod.Unturned.Module") ?? false);
            if (moduleAsset == null)
            {
                Logger.Log("Failed to find latest OpenMod.Unturned release");
                return;
            }

            Logger.Log($"Downloading {moduleAsset.AssetName}");
            var dataZip = webClient.DownloadData(moduleAsset.BrowserDownloadUrl);
            
            Logger.Log("Extracting OpenMod..");
            var modulesDirectory = OpenModInstallerPlugin.Instance.OpenModManager.ModuleDirectory;
           
            ExtractArchive(dataZip, modulesDirectory);
            Logger.Log("Successfully installed OpenMod module.");
        }

        //There can be a long talk about this made to be universal while actually not being universal.
        public void ExtractArchive(byte[] archive, string directory)
        {
            if (!Directory.Exists(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // ZipStorer dispose is broken, so using instead memoryStream dispose
            using var stream = new MemoryStream(archive);
            var zip = ZipStorer.Create(stream);
            foreach (var file in zip.ReadCentralDir())
            {
                //We dont want to leave the readme in the Modules folder do we?
                if (file.FilenameInZip == "Readme.txt")
                {
                    continue;
                }

                var path = Path.Combine(directory, Path.GetFileName(file.FilenameInZip));

                if (File.Exists(path))
                {
                    File.Delete(path);
                }

                zip.ExtractFile(file, path);
            }
        }

        public void Revert()
        {
            // Don't uninstall OpenMod if it was already installed
            if (m_OpenModInstalledAlready)
            {
                return;
            }

            Logger.Log("Uninstalling OpenMod...");

            var moduleDirectory = OpenModInstallerPlugin.Instance.OpenModManager.ModuleDirectory;
            var moduleFiles = Directory.GetFiles(moduleDirectory, "*.module", SearchOption.TopDirectoryOnly);

            // We can't delete OpenMod dll files as we might have Assembly.Load'ed them at this point
            // But we can delete the .module files instead
            foreach (var file in moduleFiles)
            {
                File.Delete(file);
            }
        }

        private class GitHubRelease
        {
            [JsonProperty("assets")]
            public List<GitHubAsset> Assets { get; set; }
        }

        private class GitHubAsset
        {
            [JsonProperty("name")]
            public string AssetName { get; set; }

            [JsonProperty("browser_download_url")]
            public string BrowserDownloadUrl { get; set; }
        }
    }
}