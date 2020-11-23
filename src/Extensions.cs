using System;
using System.Reflection;
using OpenMod.Installer.RocketMod.Jobs;

namespace OpenMod.Installer.RocketMod
{
    public static class Extensions
    {
        public static bool HasPreventAttribute(this Type type)
        {
            return type.GetCustomAttribute(typeof(PreventAttribute)) != null;
        }

        public static string GetPreventionCommand(this Type type)
        {
            if (!type.HasPreventAttribute())
                throw new ArgumentException();
            var castedAttribute = (PreventAttribute) type.GetCustomAttribute(typeof(PreventAttribute));
            return castedAttribute._commandToPrevent;
        }
    }
}