using System;
using OpenMod.Installer.RocketMod.Helpers;
using Rocket.Core.Logging;
using System.IO;
using System.Reflection;
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

        public void ExecuteMigration()
        {
            Logger.Log($"Installing package \"{m_PackageId}\"...");
            DownloadPackage();
            Logger.Log($"Installed package \"{m_PackageId}\".");
        }

        private void DownloadPackage()
        {
            var nuGetPackageManager = NuGetHelper.GetNuGetPackageManager();

            const bool allowPrereleaseVersions = false;

            var queryPackageExactMethod = nuGetPackageManager.GetType().GetMethod("QueryPackageExactAsync", BindingFlags.Instance | BindingFlags.Public);
            var queryPackageExactTask = queryPackageExactMethod.Invoke(nuGetPackageManager, new object[] { m_PackageId, null /* version*/, allowPrereleaseVersions });
            var pluginPackage = queryPackageExactTask.GetPropertyValue("Result");
            if (pluginPackage == null)
            {
                Logger.Log($"Downloading has failed for {m_PackageId}: {NuGetInstallCode.PackageOrVersionNotFound}");
                return;
            }

            var identity = pluginPackage.GetPropertyValue("Identity");
         
            var installMethod = nuGetPackageManager.GetType().GetMethod("InstallAsync", BindingFlags.Public | BindingFlags.Instance);
            var installMethodTask = installMethod.Invoke(nuGetPackageManager, new[] { identity, allowPrereleaseVersions });
            var installResult = installMethodTask.GetType().GetPropertyValue("Result");
            var installResultCode = (NuGetInstallCode)installResult.GetPropertyValue("Code");
            if (installResultCode == NuGetInstallCode.Success || installResultCode == NuGetInstallCode.NoUpdatesFound)
            {
                m_PackageDirectory = Path.Combine(OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory, "packages", identity.ToString());
                Logger.Log($"Finished downloading \"{m_PackageId}\".");
            }
            else
            {
                Logger.Log($"Downloading has failed for {m_PackageId}: {installResultCode}");
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

        private enum NuGetInstallCode
        {
            Success,
            NoUpdatesFound,
            PackageOrVersionNotFound
        }
    }
}