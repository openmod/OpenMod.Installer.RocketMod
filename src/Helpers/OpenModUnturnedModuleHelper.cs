using OpenMod.Unturned.Module;
using SDG.Framework.Modules;
using System;
using System.Collections.Generic;
using System.Reflection;
using Module = SDG.Framework.Modules.Module;

namespace OpenMod.Installer.RocketMod.Helpers
{
    public static class OpenModUnturnedModuleHelper
    {
        private static OpenModUnturnedModule m_Instance;

        public static OpenModUnturnedModule GetOpenModModule()
        {
            if (m_Instance != null)
            {
                return m_Instance;
            }

            var module = ModuleHook.getModuleByName("OpenMod.Unturned");
            var nexii = (IList<IModuleNexus>)typeof(Module).GetField("nexii", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(module);
            m_Instance = (OpenModUnturnedModule)nexii[0];

            return m_Instance;
        }
    }
}
