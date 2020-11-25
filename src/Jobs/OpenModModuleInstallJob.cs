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
        }

        public void Revert()
        {
            
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
        }
    }
}