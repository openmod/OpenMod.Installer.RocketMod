using System.Reflection;
using System.Threading.Tasks;

namespace OpenMod.Installer.RocketMod.Helpers.Wrapper
{
    public class NuGetPackageManagerWrapper : TypeWrapper
    {
        protected static PropertyInfo m_Logger;
        protected static MethodInfo m_IgnoreDependencies;
        protected static MethodInfo m_GetLatestPackageIdentityAsync;
        protected static MethodInfo m_QueryPackageExactAsync;
        protected static MethodInfo m_InstallAsync;

        public NuGetPackageManagerWrapper(Assembly nugetAssembly, params object[] args) : base(nugetAssembly, "NuGetPackageManager", args)
        {
            m_Logger ??= s_Type.GetProperty("Logger");
            m_IgnoreDependencies ??= s_Type.GetMethod("IgnoreDependencies");
            m_GetLatestPackageIdentityAsync ??= s_Type.GetMethod("GetLatestPackageIdentityAsync");
            m_QueryPackageExactAsync ??= s_Type.GetMethod("QueryPackageExactAsync");
            m_InstallAsync ??= s_Type.GetMethod("InstallAsync");
        }

        public void SetLogger(NuGetConsoleLoggerWrapper logger)
        {
            m_Logger.SetValue(m_Instance, logger.Instance);
        }

        public void IgnoreDependencies(params string[] packageIds)
        {
            m_IgnoreDependencies.Invoke(m_Instance, new object[] { packageIds });
        }

        public async Task<PackageIdentityWrapper> GetLatestPackageIdentityAsync(string packageId)
        {
            var taskObj = m_GetLatestPackageIdentityAsync.Invoke(m_Instance, new object[] { packageId }) as Task;
            var taskWrapper = new TaskWrapper<PackageIdentityWrapper>(taskObj);
            return await taskWrapper.GetResult();
        }

        public async Task<PackageSearchMetadataWrapper> QueryPackageExactAsync(string packageId, string version = null, bool includePreRelease = false)
        {
            var taskObj = m_QueryPackageExactAsync.Invoke(m_Instance, new object[] { packageId, version, includePreRelease }) as Task;
            var taskWrapper = new TaskWrapper<PackageSearchMetadataWrapper>(taskObj);
            return await taskWrapper.GetResult();
        }

        public async Task<NuGetInstallResultWrapper> InstallAsync(PackageIdentityWrapper packageIdentity, bool allowPreReleaseVersions = false)
        {
            var taskObj = m_InstallAsync.Invoke(m_Instance, new[] { packageIdentity.Instance, allowPreReleaseVersions }) as Task;
            var taskWrapper = new TaskWrapper<NuGetInstallResultWrapper>(taskObj);
            return await taskWrapper.GetResult();
        }
    }
}