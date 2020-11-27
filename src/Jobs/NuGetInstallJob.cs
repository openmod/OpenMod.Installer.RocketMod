using NuGet.Packaging.Core;
using NuGet.Protocol.Core.Types;
using OpenMod.Installer.RocketMod.Helpers;
using OpenMod.NuGet;
using Rocket.Core.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public abstract class NuGetInstallJob : IJob, IRevertable
    {
        private readonly string _packageId;
        private PackageIdentity _packageIdentity;

        protected NuGetInstallJob(string packageId)
        {
            _packageId = packageId;
        }

        public void ExecuteMigration()
        {
            AsyncHelper.RunSync(DownloadPackage);
        }

        private async Task DownloadPackage()
        {
            var nuGetPackageManager = NuGetHelper.GetNuGetPackageManager();

            const bool allowPrereleaseVersions = false;

            var packageIdentity = await nuGetPackageManager.GetLatestPackageIdentityAsync(_packageId);
            var shouldInstallOrUpdate = packageIdentity == null;

            IPackageSearchMetadata pluginPackage = null;
            if (packageIdentity == null)
            {
                pluginPackage = await nuGetPackageManager.QueryPackageExactAsync(_packageId, null, allowPrereleaseVersions);
            }

            if (packageIdentity != null)
            {
                var availableVersions = await pluginPackage.GetVersionsAsync();
                shouldInstallOrUpdate = availableVersions.Any(d => d.Version > packageIdentity.Version);
            }

            if (shouldInstallOrUpdate)
            {
                var installResult = await nuGetPackageManager.InstallAsync(pluginPackage.Identity, allowPrereleaseVersions);
                if (installResult.Code == NuGetInstallCode.Success)
                {
                    _packageIdentity = installResult.Identity;
                    Logger.Log($"Finished downloading \"{_packageId}\".");
                }
                else
                {
                    Logger.Log($"Downloading has failed for {pluginPackage.Identity.Id} v{pluginPackage.Identity.Version.OriginalVersion}: {installResult.Code}");
                }
            }
        }

        public void Revert()
        {
            if(_packageIdentity == null)
            {
                return;
            }

            var nuGetPackageManager = NuGetHelper.GetNuGetPackageManager();
            AsyncHelper.RunSync(() => nuGetPackageManager.RemoveAsync(_packageIdentity));
        }
    }
}