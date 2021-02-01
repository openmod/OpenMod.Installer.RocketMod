using System;
using System.IO;
using System.Reflection;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class NuGetHelper
    {
        private static object m_NuGetPackageManager;
        public static object GetNuGetPackageManager()
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

            var assembly = AssemblyHelper.GetAssembly("OpenMod.NuGet");
            var nugetPackageManagerType = assembly.GetType("OpenMod.NuGet.NuGetPackageManager");
            var ignoreDependenciesMethod = nugetPackageManagerType.GetMethod("IgnoreDependencies", BindingFlags.Instance | BindingFlags.Public);
            m_NuGetPackageManager = Activator.CreateInstance(nugetPackageManagerType, packagesPath);

            ignoreDependenciesMethod.Invoke(m_NuGetPackageManager, new object[]
            {
                new[]
                {
                    "Microsoft.NETCore.Platforms",
                    "Microsoft.Packaging.Tools",
                    "NETStandard.Library",
                    "OpenMod.Unturned.Redist",
                    "OpenMod.UnityEngine.Redist",
                    "System.IO.FileSystem.Watcher"
                }
            });

            return m_NuGetPackageManager;
        }
    }
}
