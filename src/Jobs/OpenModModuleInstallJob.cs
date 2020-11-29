using Newtonsoft.Json;
using OpenMod.Installer.RocketMod.Models;
using Rocket.Core.Logging;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class OpenModModuleInstallJob : IJob, IRevertable
    {
        public void ExecuteMigration()
        {
            using var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "request");
            var releaseData = webClient.DownloadString("https://api.github.com/repos/openmod/openmod/releases/latest");
            var release = JsonConvert.DeserializeObject<LatestRelease>(releaseData);

            //this can never be empty UNLESS troja posts a release without Module
            var moduleAsset = release.Assets.Find(x => x.BrowserDownloadUrl.Contains("OpenMod.Unturned.Module"));
            Logger.Log($"Downloading {moduleAsset.AssetName}");
            var dataZip = webClient.DownloadData(moduleAsset.BrowserDownloadUrl);
            Logger.Log("Extracting..");
            var modulesDirectory = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory;
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
                    continue;
                var path = Path.Combine(directory, Path.GetFileName(file.FilenameInZip));
                zip.ExtractFile(file, path);
            }
        }

        public void Revert()
        {
            if (Directory.Exists(OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory))
                Directory.Delete(OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory, true);
        }
    }
}