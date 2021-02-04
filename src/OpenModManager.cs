using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using SDG.Framework.Modules;
using SDG.Unturned;

namespace OpenMod.Installer.RocketMod
{
    public class OpenModManager
    {
        private static IModuleNexus s_Instance;

        public OpenModManager()
        {
            WorkingDirectory = Path.Combine(ReadWrite.PATH, "Servers", Provider.serverID, "OpenMod");

            // Find existing module directory. It may not be named OpenMod.Unturned.
            var modulesDirectory = Path.Combine(ReadWrite.PATH, "Modules");
            foreach (var moduleDirectory in Directory.GetDirectories(modulesDirectory))
            {
                var isModuleFilePresent = Directory
                    .GetFiles(moduleDirectory, "*.module", SearchOption.TopDirectoryOnly)
                    .Length > 0;

                // Not an Unturned module
                if (!isModuleFilePresent)
                {
                    continue;
                }

                foreach (var file in Directory.GetFiles(moduleDirectory, "*.dll", SearchOption.TopDirectoryOnly))
                {
                    var assemblyName = AssemblyName.GetAssemblyName(file);
                    if (assemblyName.Name.Equals("OpenMod.Unturned"))
                    {
                        InstalledVersion = assemblyName.Version;
                        ModuleDirectory = moduleDirectory;
                        return;
                    }
                }
            }

            // OpenMod is not installed; set to default module directory
            ModuleDirectory = Path.Combine(modulesDirectory, "OpenMod.Unturned");
        }

        /// <summary>
        /// Gets an OpenMod assembly.
        /// </summary>
        /// <param name="name">The name of the assembly, e.g. OpenMod.Core, OpenMod.Unturned.</param>
        /// <returns>The OpenMod assembly</returns>
        public Assembly GetOpenModAssembly(string name)
        {
            if (!IsOpenModInstalled)
            {
                throw new Exception("OpenMod is not installed.");
            }

            foreach (var file in Directory.GetFiles(ModuleDirectory, "*.dll", SearchOption.TopDirectoryOnly))
            {
                var assemblyName = AssemblyName.GetAssemblyName(file);
                if (assemblyName.Name.Equals(name))
                {
                    return Assembly.Load(File.ReadAllBytes(file));
                }
            }

            throw new Exception($"Unknown OpenMod assembly \"{name}\", search directory: {ModuleDirectory}");
        }

        /// <summary>
        /// Checks if OpenMod is loaded as a module.
        /// </summary>
        public bool IsOpenModModuleLoaded
        {
            get => GetOpenModModule() != null;
        }

        /// <summary>
        /// Gets the installed OpenMod version.
        /// </summary>
        public Version InstalledVersion { get; }

        /// <summary>
        /// Gets the OpenMod working directory.
        /// </summary>
        public string WorkingDirectory { get; }

        /// <summary>
        /// Gets the OpenMod module directory.
        /// </summary>
        public string ModuleDirectory { get; }

        /// <summary>
        /// Checks if OpenMod is installed. Keep in mind that OpenMod can be installed but the module may be disabled.
        /// Use <see cref="IsOpenModModuleLoaded"/> to check if OpenMod is loaded.
        /// </summary>
        public bool IsOpenModInstalled
        {
            get => InstalledVersion != null;
        }

        /// <summary>
        /// Gets the OpenMod Unturnedm odule
        /// </summary>
        /// <returns>The OpenMod Unturned module, can return null if the module is not loaded.</returns>
        public static IModuleNexus GetOpenModModule()
        {
            if (s_Instance != null)
            {
                return s_Instance;
            }

            var module = ModuleHook.getModuleByName("OpenMod.Unturned");
            var field = typeof(SDG.Framework.Modules.Module).GetField("nexii", BindingFlags.NonPublic | BindingFlags.Instance);
            if (field == null)
            {
                throw new Exception("Failed to find nexii field in SDG.Unturned.Module class");
            }

            var nexii = (IList<IModuleNexus>)field.GetValue(module);
            s_Instance = nexii.FirstOrDefault(d => d.GetType().Assembly.FullName.Contains("OpenMod"));

            return s_Instance;
        }
    }
}