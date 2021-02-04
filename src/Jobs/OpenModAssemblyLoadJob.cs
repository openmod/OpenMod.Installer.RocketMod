using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class OpenModAssemblyLoadJob : IReversibleJob
    {
        private static bool s_AssemblyResolverInstalled;
        private static readonly Dictionary<string, Assembly> s_LoadedAssemblies = new Dictionary<string, Assembly>();
        private static readonly Regex s_VersionRegex = new Regex("Version=(?<version>.+?), ", RegexOptions.Compiled);

        public void ExecuteMigration(string[] args)
        {
            Logger.Log("Loading OpenMod assemblies. Ignore the message spam below.");
            var aviEvents = new List<AssemblyLoadEventHandler>();
            try
            {
                var assemblyLoadEvent = (MulticastDelegate)typeof(AppDomain)
                    .GetField("AssemblyLoad", BindingFlags.NonPublic | BindingFlags.Instance)!
                    .GetValue(AppDomain.CurrentDomain);

                if (assemblyLoadEvent != null)
                {
                    foreach (var @delegate in assemblyLoadEvent.GetInvocationList())
                    {
                        var asmFullName = @delegate?.GetMethodInfo()?.DeclaringType?.Assembly?.FullName;
                        if (asmFullName != null && (asmFullName.StartsWith("AviRockets.Module", StringComparison.OrdinalIgnoreCase)
                            || asmFullName.StartsWith("AviRockets.Mothership", StringComparison.OrdinalIgnoreCase)))
                        {
                            var eventHandler = (AssemblyLoadEventHandler)@delegate;
                            AppDomain.CurrentDomain.AssemblyLoad -= eventHandler;
                            aviEvents.Add(eventHandler);
                        }
                    }
                }

                if (!s_AssemblyResolverInstalled)
                {
                    AppDomain.CurrentDomain.AssemblyResolve += OnAssemblyResolve;
                    s_AssemblyResolverInstalled = true;
                }

                var moduleDirectory = OpenModInstallerPlugin.Instance.OpenModManager.ModuleDirectory;
                foreach (var dllpath in Directory.GetFiles(moduleDirectory, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    var asm = Assembly.Load(File.ReadAllBytes(dllpath));

                    var name = GetVersionIndependentName(asm.FullName);
                    if (s_LoadedAssemblies.ContainsKey(name))
                    {
                        continue;
                    }

                    s_LoadedAssemblies.Add(name, asm);
                }
            }
            finally
            {
                foreach (var @event in aviEvents)
                {
                    AppDomain.CurrentDomain.AssemblyLoad += @event;
                }
            }
        }

        private Assembly OnAssemblyResolve(object sender, ResolveEventArgs args)
        {
            var name = GetVersionIndependentName(args.Name);

            if (s_LoadedAssemblies.ContainsKey(name))
            {
                return s_LoadedAssemblies[name];
            }

            return null;
        }

        public static string GetVersionIndependentName(string fullAssemblyName)
        {
            return s_VersionRegex.Replace(fullAssemblyName, string.Empty);
        }

        public void Revert()
        {
            s_LoadedAssemblies.Clear();

            if (s_AssemblyResolverInstalled)
            {
                AppDomain.CurrentDomain.AssemblyResolve -= OnAssemblyResolve;
                s_AssemblyResolverInstalled = false;
            }
        }
    }
}
