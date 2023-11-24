using System;
using System.IO;
using System.Linq;
using OpenMod.Installer.RocketMod.Helpers.Wrapper;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class NuGetHelper
    {
        private static NuGetPackageManagerWrapper s_NuGetPackageManagerWrapper;

        public static NuGetPackageManagerWrapper GetNuGetPackageManager()
        {
            if (s_NuGetPackageManagerWrapper != null)
            {
                return s_NuGetPackageManagerWrapper;
            }

            var workingDirectory = OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory;
            var packagesPath = Path.Combine(workingDirectory, "packages");

            if (!Directory.Exists(packagesPath))
            {
                Directory.CreateDirectory(packagesPath);
            }

            Environment.SetEnvironmentVariable("NUGET_COMMON_APPLICATION_DATA", packagesPath);

            var nugetAssembly = AppDomain.CurrentDomain.GetAssemblies().First(a => a.GetName().Name.Equals("OpenMod.Nuget", StringComparison.OrdinalIgnoreCase));

            s_NuGetPackageManagerWrapper = new NuGetPackageManagerWrapper(nugetAssembly, packagesPath);

            var logger = new NuGetConsoleLoggerWrapper(nugetAssembly);
            s_NuGetPackageManagerWrapper.SetLogger(logger);

            s_NuGetPackageManagerWrapper.IgnoreDependencies(
                "Microsoft.NETCore.Platforms",
                "Microsoft.Packaging.Tools",
                "NETStandard.Library",
                /*"OpenMod.Unturned.Redist",
                "OpenMod.UnityEngine.Redist",*/ // todo
                "System.IO.FileSystem.Watcher");

            return s_NuGetPackageManagerWrapper;
        }
    }
}
