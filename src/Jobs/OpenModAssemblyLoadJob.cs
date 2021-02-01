using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using OpenMod.Installer.RocketMod.Helpers;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class OpenModAssemblyLoadJob : IReversibleJob
    {
        private static bool m_AssemblyResolveInstalled;
        private static readonly Dictionary<string, Assembly> m_LoadedAssembles = new Dictionary<string, Assembly>();

        public void ExecuteMigration()
        {
            if (!m_AssemblyResolveInstalled)
            {
                AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
                m_AssemblyResolveInstalled = true;
            }

            foreach (var file in Directory.GetFiles(OpenModInstallerPlugin.Instance.OpenModManager.ModuleDirectory))
            {
                if (file.EndsWith(".dll"))
                {
                    var dllPath = Path.Combine(OpenModInstallerPlugin.Instance.OpenModManager.ModuleDirectory, file);
                    var asm = Assembly.Load(File.ReadAllBytes(dllPath));

                    var name = Extensions.GetVersionIndependentName(asm.FullName, out _);
                    if (m_LoadedAssembles.ContainsKey(name))
                    {
                        continue;
                    }

                    m_LoadedAssembles.Add(name, asm);
                }
            }
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = Extensions.GetVersionIndependentName(args.Name, out _);

            if (m_LoadedAssembles.ContainsKey(name))
            {
                return m_LoadedAssembles[name];
            }

            return null;
        }

        public void Revert()
        {
            if (m_AssemblyResolveInstalled)
            {
                m_LoadedAssembles.Clear();
                AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
                m_AssemblyResolveInstalled = false;
            }
        }
    }
}
