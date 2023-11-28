using System.Reflection;

namespace OpenMod.Installer.RocketMod.Helpers.Wrapper
{
    public class PackageSearchMetadataWrapper : TypeWrapper
    {
        protected static PropertyInfo m_Identity;

        public PackageIdentityWrapper Identity
        {
            get
            {
                var identity = m_Identity.GetValue(m_Instance);
                return new PackageIdentityWrapper(identity);
            }
        }

        public PackageSearchMetadataWrapper(object instance) : base(instance)
        {
            m_Identity = s_Type.GetProperty("Identity");
        }
    }
}