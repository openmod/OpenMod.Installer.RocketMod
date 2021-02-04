using OpenMod.Installer.RocketMod.Helpers;
using OpenMod.NuGet;
using Rocket.Core.Logging;
using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public abstract class NuGetInstallJob : IReversibleJob
    {
        private readonly string m_PackageId;
        private string m_PackageDirectory;

        protected NuGetInstallJob(string packageId)
        {
            m_PackageId = packageId;
        }

        public void ExecuteMigration(string[] args)
        {
            AsyncHelperEx.RunSync(() => DownloadPackage(args.Contains("--force") || args.Contains("-f")));
        }

        private async Task DownloadPackage(bool force)
        {
            Logger.Log($"Installing package \"{m_PackageId}\"...");
            var nuGetPackageManager = NuGetHelper.GetNuGetPackageManager();

            const bool c_AllowPreReleaseVersion = false;

            var oldIdentity = await nuGetPackageManager.GetLatestPackageIdentityAsync(m_PackageId);
            if (!force && oldIdentity != null)
            {
                Logger.LogWarning($"Package \"{m_PackageId}\" is already installed, skipping. Use \"openmod install -f\" to install anyway.");
                return;
            }

            var package = await nuGetPackageManager.QueryPackageExactAsync(m_PackageId, null, c_AllowPreReleaseVersion);
            if (package?.Identity == null)
            {
                Logger.LogError($"Downloading has failed for {m_PackageId}: {NuGetInstallCode.PackageOrVersionNotFound}");
                return;
            }

            if (oldIdentity?.Version == package.Identity.Version && package.Identity.HasVersion)
            {
                Logger.LogError($"Latest version of {package.Identity.Id} is already installed.");
                return;
            }

            var identity = package.Identity;

            var installResult = await nuGetPackageManager.InstallAsync(identity, c_AllowPreReleaseVersion);
            bool isInstalled;
            if (Enum.GetValues(typeof(NuGetInstallCode)).Length == 3) // loaded 3.0 version openmod 
            {
                isInstalled = (int) installResult.Code is 0 or 1; // Success / NoUpdatesFound
            }
            else
            {
                isInstalled = installResult.Code == NuGetInstallCode.Success;
            }

            if (isInstalled)
            {
                m_PackageDirectory = Path.Combine(OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory,
                    "packages",
                    identity.ToString());
                Logger.Log($"Finished downloading \"{m_PackageId}\".");
            }
            else
            {
                Logger.Log($"Downloading has failed for {m_PackageId}: {installResult.Code}");
            }

            Logger.Log($"Installed package \"{m_PackageId}\"@{identity.Version}.");
        }

        public void Revert()
        {
            if (string.IsNullOrEmpty(m_PackageDirectory) || !Directory.Exists(m_PackageDirectory))
            {
                return;
            }

            Logger.Log($"Uninstalling package \"{m_PackageId}\"");
            Directory.Delete(m_PackageDirectory, true);
        }
    }
}