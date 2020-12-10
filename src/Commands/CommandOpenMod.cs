using Rocket.API;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpenMod.Installer.RocketMod.Commands
{
    public class CommandOpenMod : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            var isOpenmod = OpenModInstallerPlugin.Instance.IsOpenModRocketModBridge;
            if(command.Length == 0 || (!string.Equals(command[0], "uninstall", StringComparison.OrdinalIgnoreCase)
                && !string.Equals(command[0], "install", StringComparison.OrdinalIgnoreCase)))
            {
                Logger.Log($"Unsupported syntax. Use /openmod {(isOpenmod ? "uninstall" : "install")} [flags]");
                return;
            }

            if(!isOpenmod && string.Equals(command[0], "uninstall", StringComparison.OrdinalIgnoreCase))
            {
                Logger.Log("OpenMod is not installed!");
                return;
            }
            else if (isOpenmod && string.Equals(command[0], "install", StringComparison.OrdinalIgnoreCase))
            {
                Logger.Log("OpenMod already installed!");
                return;
            }

            var flags = command.Skip(1).Select(x => x.ToLower());
            OpenModInstallerPlugin.Instance.JobsManager.Migrate(flags.ToArray());
        }

        public AllowedCaller AllowedCaller { get; } = AllowedCaller.Console;
        public string Name { get; } = "openmod";
        public string Help { get; } = "Installs OpenMod";
        public string Syntax { get; } = "<install|uninstall>";
        public List<string> Aliases { get; } = new List<string>();
        public List<string> Permissions { get; } = new List<string>();
    }
}