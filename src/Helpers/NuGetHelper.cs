using OpenMod.Installer.RocketMod.Jobs;
using OpenMod.NuGet;
using System;
using System.IO;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class NuGetHelper
    {
        private static NuGetPackageManager s_NuGetPackageManager;

        public static NuGetPackageManager GetNuGetPackageManager()
        {
            if (s_NuGetPackageManager != null)
            {
                return s_NuGetPackageManager;
            }

            var workingDirectory = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory;
            var packagesPath = Path.Combine(workingDirectory, "packages");

            if (!Directory.Exists(packagesPath))
            {
                Directory.CreateDirectory(packagesPath);
            }

            Environment.SetEnvironmentVariable("NUGET_COMMON_APPLICATION_DATA", packagesPath);

            s_NuGetPackageManager = new NuGetPackageManager(packagesPath)
            {
                Logger = new NuGetConsoleLogger()
            };

            s_NuGetPackageManager.IgnoreDependencies(
                "Microsoft.NETCore.Platforms",
                "Microsoft.Packaging.Tools",
                "NETStandard.Library",
                /*"OpenMod.Unturned.Redist",
                "OpenMod.UnityEngine.Redist",*/ // todo
                "System.IO.FileSystem.Watcher");

            return s_NuGetPackageManager;
        }
    }
}
