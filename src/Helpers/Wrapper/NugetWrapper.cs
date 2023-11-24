using System.Reflection;

namespace OpenMod.Installer.RocketMod.Helpers.Wrapper
{
    public class NuGetInstallResultWrapper : TypeWrapper
    {
        protected static PropertyInfo m_Code;

        public int Code => (int)m_Code.GetValue(m_Instance);

        public NuGetInstallResultWrapper(object instance) : base(instance)
        {
            m_Code = s_Type.GetProperty("Code");
        }
    }
}
