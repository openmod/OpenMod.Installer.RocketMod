using OpenMod.NuGet;
using System;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class NuGetHelper
    {
        private static NuGetPackageManager NuGetPackageManager { get; set; }
        public static NuGetPackageManager GetNuGetPackageManager()
        {
            if (NuGetPackageManager != null)
            {
                return NuGetPackageManager;
            }

            var path = OpenModInstallerPlugin.Instance.OpenModManager.PackagesDirectory;
            Environment.SetEnvironmentVariable("NUGET_COMMON_APPLICATION_DATA", path);

            // NuGetPackageManager should auto create directory
            NuGetPackageManager = new NuGetPackageManager(path)
            {
                Logger = new NuGetConsoleLogger()
            };

            // these dependencies do not exist on NuGet and create warnings
            // they are not required
            NuGetPackageManager.IgnoreDependencies(
                "Microsoft.NETCore.Platforms",
                "Microsoft.Packaging.Tools",
                "NETStandard.Library",
                "System.IO.FileSystem.Watcher");

            return NuGetPackageManager;
        }
    }
}
