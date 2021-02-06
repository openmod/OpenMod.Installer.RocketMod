using System;
using System.Reflection;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class AssemblyHelper
    {
        public static Assembly GetAssembly(string name)
        {
            return Array.Find(AppDomain.CurrentDomain.GetAssemblies(), d => d.GetName().Name.Equals(name))
                   ?? throw new Exception($"Assembly not found: {name}");
        }
    }
}