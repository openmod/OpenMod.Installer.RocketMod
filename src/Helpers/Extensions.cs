using System.Text.RegularExpressions;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class Extensions
    {
        private static readonly Regex VersionRegex = new Regex("Version=(?<version>.+?), ", RegexOptions.Compiled);

        public static string GetVersionIndependentName(string fullAssemblyName, out string extractedVersion)
        {
            var match = VersionRegex.Match(fullAssemblyName);
            extractedVersion = match.Groups[1].Value;
            return VersionRegex.Replace(fullAssemblyName, string.Empty);
        }
    }
}