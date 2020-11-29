using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class AssemblyLoadJob : IJob, IRevertable
    {
        private static bool _assemblyResolveInstalled;
        private static readonly Dictionary<string, Assembly> _loadedAssembles = new Dictionary<string, Assembly>();

        public void ExecuteMigration()
        {
            if (!_assemblyResolveInstalled)
            {
                AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
                _assemblyResolveInstalled = true;
            }

            foreach (var file in Directory.GetFiles(OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory))
            {
                if (file.EndsWith(".dll"))
                {
                    var dllPath = Path.Combine(OpenModInstallerPlugin.Instance.OpenModManager.WorkingDirectory, file);
                    var asm = Assembly.Load(File.ReadAllBytes(dllPath));

                    var name = Extensions.GetVersionIndependentName(asm.FullName, out _);
                    if (_loadedAssembles.ContainsKey(name))
                    {
                        continue;
                    }

                    _loadedAssembles.Add(name, asm);
                }
            }
        }

        private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = Extensions.GetVersionIndependentName(args.Name, out _);
            if (_loadedAssembles.ContainsKey(name))
            {
                return _loadedAssembles[name];
            }
            return null;
        }

        public void Revert()
        {
            if (_assemblyResolveInstalled)
            {
                _loadedAssembles.Clear();
                AppDomain.CurrentDomain.AssemblyResolve -= CurrentDomain_AssemblyResolve;
                _assemblyResolveInstalled = false;
            }
        }
    }
}
