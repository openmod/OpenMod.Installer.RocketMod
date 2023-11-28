using System;
using System.Linq;
using System.Reflection;

namespace OpenMod.Installer.RocketMod.Helpers.Wrapper
{
    public abstract class TypeWrapper
    {
        protected Type s_Type;

        protected object m_Instance;

        public object Instance => m_Instance;

        protected TypeWrapper(object instance)
        {
            m_Instance = instance;
            s_Type = instance.GetType();
        }

        protected TypeWrapper(Assembly nugetAssembly, string typeName, params object[] args)
        {
            s_Type = nugetAssembly.GetTypes().First(tp => tp.Name.Equals(typeName, StringComparison.OrdinalIgnoreCase));
            m_Instance = Activator.CreateInstance(s_Type, args);
        }
    }
}