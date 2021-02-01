using OpenMod.Installer.RocketMod.Helpers;
using OpenMod.NuGet;
using Rocket.Core.Logging;
using System.IO;
using System.Threading.Tasks;
using OpenMod.Core.Helpers;

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

        public void ExecuteMigration()
        {
            Logger.Log($"Installing package \"{m_PackageId}\"...");
            AsyncHelper.RunSync(DownloadPackage);
        }

        private async Task DownloadPackage()
        {
            var nuGetPackageManager = NuGetHelper.GetNuGetPackageManager();

            const bool allowPrereleaseVersions = false;

            var pluginPackage = await nuGetPackageManager.QueryPackageExactAsync(m_PackageId, includePreRelease: allowPrereleaseVersions);
            if (pluginPackage == null)
            {
                Logger.Log($"Downloading has failed for {m_PackageId}: {NuGetInstallCode.PackageOrVersionNotFound}");
                return;
            }

            var installResult = await nuGetPackageManager.InstallAsync(pluginPackage.Identity, allowPrereleaseVersions);
            if (installResult.Code == NuGetInstallCode.Success) // todo there is another  code
            {
                m_PackageDirectory = Path.Combine(OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory, "packages",
                    installResult.Identity.ToString());
                Logger.Log($"Finished downloading \"{m_PackageId}\".");
            }
            else
            {
                Logger.Log($"Downloading has failed for {pluginPackage.Identity.Id} v{pluginPackage.Identity.Version.OriginalVersion}: {installResult.Code}");
            }
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