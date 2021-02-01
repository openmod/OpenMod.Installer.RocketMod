using System;
using System.Linq;
using System.Reflection;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class AssemblyHelper
    {
        public static Assembly GetAssembly(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(d => d.GetName().Name.Equals(name))
                   ?? throw new Exception($"Assembly not found: {name}");
        }
    }
}