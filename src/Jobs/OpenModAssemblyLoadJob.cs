using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using Rocket.Core.Logging;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class OpenModAssemblyLoadJob : IReversibleJob
    {
        private static bool m_AssemblyResolverInstalled;
        private static readonly Dictionary<string, Assembly> m_LoadedAssemblies = new Dictionary<string, Assembly>();
        private static readonly Regex VersionRegex = new Regex("Version=(?<version>.+?), ", RegexOptions.Compiled);

        public void ExecuteMigration()
        {
            Logger.Log("Loading OpenMod assemblies. Ignore the message spam below.");

            if (!m_AssemblyResolverInstalled)
            {
                AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
                m_AssemblyResolverInstalled = true;
            }

            var moduleDirectory = OpenModInstallerPlugin.Instance.OpenModManager.ModuleDirectory;
            foreach (var file in Directory.GetFiles(moduleDirectory, "*.dll", SearchOption.TopDirectoryOnly))
            {
                var dllPath = Path.Combine(OpenModInstallerPlugin.Instance.OpenModManager.ModuleDirectory, file);
                var asm = Assembly.Load(File.ReadAllBytes(dllPath));

                var name = GetVersionIndependentName(asm.FullName);
                if (m_LoadedAssemblies.ContainsKey(name))
                {
                    continue;
                }

                m_LoadedAssemblies.Add(name, asm);
            }
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = GetVersionIndependentName(args.Name);

            if (m_LoadedAssemblies.ContainsKey(name))
            {
                return m_LoadedAssemblies[name];
            }

            return null;
        }

        public static string GetVersionIndependentName(string fullAssemblyName)
        {
            return VersionRegex.Replace(fullAssemblyName, string.Empty);
        }

        public void Revert()
        {
            m_LoadedAssemblies.Clear();
    
            if (m_AssemblyResolverInstalled)
            {
                AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
                m_AssemblyResolverInstalled = false;
            }
        }
    }
}
