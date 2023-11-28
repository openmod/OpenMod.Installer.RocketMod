using System;
using OpenMod.Installer.RocketMod.Helpers;
using Rocket.Core.Logging;
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
            AsyncHelperEx.RunSync(() => DownloadPackage(args.Contains("--force") || args.Contains("-f"), args.Contains("-p") || args.Contains("--pre")));
        }

        private async Task DownloadPackage(bool force, bool usePre)
        {
            Logger.Log($"Installing package \"{m_PackageId}\"...");
            var nuGetPackageManager = NuGetHelper.GetNuGetPackageManager();

            var oldIdentity = await nuGetPackageManager.GetLatestPackageIdentityAsync(m_PackageId);
            if (!force && oldIdentity != null)
            {
                Logger.LogWarning($"Package \"{m_PackageId}\" is already installed, skipping. Use \"openmod install -f\" to install anyway.");
                return;
            }

            var package = await nuGetPackageManager.QueryPackageExactAsync(m_PackageId, null, usePre);
            if (package?.Identity == null)
            {
                Logger.LogError($"Downloading has failed for {m_PackageId}: PackageOrVersionNotFound");
                return;
            }

            if (oldIdentity?.Version == package.Identity.Version && package.Identity.HasVersion)
            {
                Logger.LogError($"Latest version of {package.Identity.Id} is already installed.");
                return;
            }

            var identity = package.Identity;

            var installResult = await nuGetPackageManager.InstallAsync(identity, usePre);
            var isInstalled = installResult.Code == 0 || installResult.Code == 1;

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