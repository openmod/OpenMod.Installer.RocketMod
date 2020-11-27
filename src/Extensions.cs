using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using OpenMod.Installer.RocketMod.Jobs;

namespace OpenMod.Installer.RocketMod
{
    public static class Extensions
    {
        public static bool HasPreventAttribute(this Type type)
        {
            return type.GetCustomAttributes<PreventAttribute>().Any();
        }

        public static string[] GetPreventionCommands(this Type type)
        {
            if (!type.HasPreventAttribute())
                throw new ArgumentException();
            var attributes = type.GetCustomAttributes<PreventAttribute>();
            return attributes.Select(x => x.CommandToPrevent).ToArray();
        }

        private static readonly Regex VersionRegex = new Regex("Version=(?<version>.+?), ", RegexOptions.Compiled);

        public static string GetVersionIndependentName(string fullAssemblyName, out string extractedVersion)
        {
            var match = VersionRegex.Match(fullAssemblyName);
            extractedVersion = match.Groups[1].Value;
            return VersionRegex.Replace(fullAssemblyName, string.Empty);
        }
    }
}