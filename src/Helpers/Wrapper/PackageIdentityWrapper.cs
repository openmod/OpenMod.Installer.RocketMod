using System.Reflection;

namespace OpenMod.Installer.RocketMod.Helpers.Wrapper
{
    public class PackageIdentityWrapper : TypeWrapper
    {
        protected static FieldInfo m_Id;
        protected static FieldInfo m_Version;

        public string Id => m_Id.GetValue(m_Instance) as string;

        public bool HasVersion
        {
            get
            {
                var version = m_Version.GetValue(m_Instance);
                return version != null;
            }
        }

        public NuGetVersionWrapper Version
        {
            get
            {
                var version = m_Version.GetValue(m_Instance);
                return version == null ? null : new NuGetVersionWrapper(version);
            }
        }

        public PackageIdentityWrapper(object instance) : base(instance)
        {
            m_Id = s_Type.GetField("_id", BindingFlags.NonPublic | BindingFlags.Instance);
            m_Version = s_Type.GetField("_version", BindingFlags.NonPublic | BindingFlags.Instance);
        }
    }
}