using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using Newtonsoft.Json;
using Rocket.Core.Logging;
using SDG.Unturned;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class OpenModModuleInstallJob : IJob, IRevertable
    {
        public void ExecuteMigration()
        {
<<<<<<< Updated upstream
            var client = new WebClient();
            //Returns 403: Forbidden if User-Agent is not set.
            client.Headers["User-Agent"] =
                "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/87.0.4280.66 Safari/537.36";
            var data = client.DownloadString("https://api.github.com/repos/openmod/openmod/releases?per_page=1");
            var latestrelease = JsonConvert.DeserializeObject<List<Release>>(data).First();
            var moduleAsset = latestrelease.assets.First(x => x.name.Contains("OpenMod.Unturned.Module"));
            Logger.Log($"Downloading {moduleAsset.name}..");
            var archiveBin = client.DownloadData(moduleAsset.browser_download_url);
            Logger.Log("Extracting..");
            //Gotta extract it somehow ¯\_(ツ)_/¯
            Logger.Log("Successfully installed OpenMod module.");
=======
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
>>>>>>> Stashed changes
        }

        public void Revert()
        {
<<<<<<< Updated upstream
            
        }

        public class Release
        {
            public string name;
            
            public List<Asset> assets;
        }

        public class Asset
        {
            public string name;
            public string browser_download_url;
=======
            if (Directory.Exists(OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory))
                Directory.Delete(OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory, true);
>>>>>>> Stashed changes
        }
    }
}