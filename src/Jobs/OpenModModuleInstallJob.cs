using Newtonsoft.Json;
using OpenMod.Installer.RocketMod.Models;
using Rocket.Core.Logging;
using SDG.Unturned;
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
            var webClient = new WebClient();
            webClient.Headers.Add("User-Agent", "request");

            var releaseData = webClient.DownloadString("https://api.github.com/repos/openmod/openmod/releases/latest");
            if (string.IsNullOrEmpty(releaseData))
            {
                throw new NullReferenceException("GitHub API returns empty request");
            }

            var release = JsonConvert.DeserializeObject<LatestRelease>(releaseData);
            var downloadLink = release.Assets.Find(x => x.BrowserDownloadUrl.Contains("OpenMod.Unturned.Module"))?.BrowserDownloadUrl;
            if (downloadLink == null)
            {
                throw new NullReferenceException(nameof(downloadLink));
            }

            Logger.Log($"Downloading OpenMod.Unturned.Module..");
            var dataZip = webClient.DownloadData(downloadLink);

            Logger.Log("Extracting..");
            using MemoryStream stream = new MemoryStream(dataZip);
            var zip = ZipStorer.Create(stream);
            foreach (var file in zip.ReadCentralDir())
            {
                var path = Path.Combine(OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory,
                    Path.GetFileName(file.FilenameInZip));
                zip.ExtractFile(file, path);
            }

            var rocketModulePath = Path.Combine(ReadWrite.PATH, "Modules", "Rocket.Unturned", "Rocket.Unturned.module");
            var renamedRocketModulePath = Path.Combine(ReadWrite.PATH, "Modules", "Rocket.Unturned", "Rocket.Unturned.module.bak");
            if(File.Exists(rocketModulePath))
            {
                File.Move(rocketModulePath, renamedRocketModulePath);
            }
            Logger.Log("Successfully installed OpenMod module.");
        }

        public void Revert()
        {
        }
    }
}