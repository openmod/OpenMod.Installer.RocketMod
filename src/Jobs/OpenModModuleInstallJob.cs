using Newtonsoft.Json;
using OpenMod.Installer.RocketMod.Helpers;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class OpenModModuleInstallJob : IReversibleJob
    {
        private bool m_OpenModInstalledAlready;

        public void ExecuteMigration(string[] args)
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
            var moduleAsset = release.Assets.Find(x => x.BrowserDownloadUrl.Contains("OpenMod.Unturned.Module"));
            if (moduleAsset == null)
            {
                Logger.Log("Failed to find latest OpenMod.Unturned release");
                return;
            }

            Logger.Log($"Downloading {moduleAsset.AssetName}");
            var dataZip = webClient.DownloadData(moduleAsset.BrowserDownloadUrl);

            Logger.Log("Extracting OpenMod module...");
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
                try
                {
                    if (File.Exists(path))
                    {
                        File.Delete(path);
                    }

                    zip.ExtractFile(file, path);
                }
                catch (Exception ex)
                {
                    Logger.LogWarning($"Failed to extract {file.FilenameInZip}: {ex.Message}.");
                }
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
            try
            {
                Directory.Delete(moduleDirectory, true);
                Logger.Log("Uninstalled OpenMod.");
            }
            catch (Exception ex)
            {
                // Delete failed but we can still delete the .module file to disable OpenMod

                Logger.LogWarning($"Failed to delete the OpenMod: {ex.Message}.");

                var moduleFiles = Directory.GetFiles(moduleDirectory, "*.module", SearchOption.TopDirectoryOnly);
                foreach (var file in moduleFiles)
                {
                    File.Delete(file);
                }
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