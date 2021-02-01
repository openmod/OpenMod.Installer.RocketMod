using OpenMod.NuGet;
using System;
using System.IO;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class NuGetHelper
    {
        private static NuGetPackageManager m_NuGetPackageManager;
        public static NuGetPackageManager GetNuGetPackageManager()
        {
            if (m_NuGetPackageManager != null)
            {
                return m_NuGetPackageManager;
            }

            var workingDirectory = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory;
            var packagesPath = Path.Combine(workingDirectory, "packages");

            if (!Directory.Exists(packagesPath))
            {
                Directory.CreateDirectory(packagesPath);
            }

            Environment.SetEnvironmentVariable("NUGET_COMMON_APPLICATION_DATA", packagesPath);

            // NuGetPackageManager should auto create directory
            m_NuGetPackageManager = new NuGetPackageManager(packagesPath)
            {
                Logger = new NuGetConsoleLogger()
            };

            m_NuGetPackageManager.IgnoreDependencies(
                "Microsoft.NETCore.Platforms",
                "Microsoft.Packaging.Tools",
                "NETStandard.Library",
                "OpenMod.Unturned.Redist",
                "OpenMod.UnityEngine.Redist",
                "System.IO.FileSystem.Watcher");

            return m_NuGetPackageManager;
        }
    }
}
