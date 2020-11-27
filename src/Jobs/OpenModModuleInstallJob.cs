using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using OpenMod.Installer.RocketMod.Models;
using Rocket.Core.Logging;
using SDG.Unturned;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class OpenModModuleInstallJob : IJob, IRevertable
    {
        public void ExecuteMigration()
        {
            var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "request");
            var releaseData = webClient.DownloadString("https://api.github.com/repos/openmod/openmod/releases/latest");
            //can it even possibly be null or empty? Gonna leave this here for now
            if (string.IsNullOrEmpty(releaseData))
            {
                throw new NullReferenceException("GitHub API returns empty response");
            }
            var release = JsonConvert.DeserializeObject<LatestRelease>(releaseData);
            //this can never be empty UNLESS troja posts a release without Module
            var moduleAsset = release.Assets.Find(x => x.BrowserDownloadUrl.Contains("OpenMod.Unturned.Module"));
            Logger.Log($"Downloading {moduleAsset.AssetName}");
            var dataZip = webClient.DownloadData(moduleAsset.BrowserDownloadUrl);
            Logger.Log("Extracting..");
            var modulesDirectory = Path.Combine(ReadWrite.PATH, "Modules");
            ExtractArchive(dataZip, modulesDirectory);
            Logger.Log("Successfully installed OpenMod module.");
        }

        //There can be a long talk about this made to be universal while actually not being universal.
        public void ExtractArchive(byte[] archive, string directory)
        {
            if (!Directory.Exists(directory))
                Directory.CreateDirectory(directory);

            var zip = ZipStorer.Create(new MemoryStream(archive));
            foreach (var file in zip.ReadCentralDir())
            {
                //We dont want to leave the readme in the Modules folder do we?
                if (file.FilenameInZip == "Readme.txt")
                    continue;
                var path = Path.Combine(directory, file.FilenameInZip);
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