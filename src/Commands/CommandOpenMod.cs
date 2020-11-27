using System.Collections.Generic;
using System.Linq;
using Rocket.API;
using Rocket.Core.Logging;

namespace OpenMod.Installer.RocketMod.Commands
{
    public class CommandOpenMod : IRocketCommand
    {
        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length == 0 || command[0].ToLower() != "install")
            {
                Logger.Log("Unsupported syntax. Use /openmod install [flags]");
                return;
            }

            if (OpenModInstallerPlugin.Instance.IsOpenModRocketModBridge)
            {
                Logger.Log("You are already running OpenMod.");
                return;
            }

            var flags = command.Skip(1).Select(x => x.ToLower());
            
            OpenModInstallerPlugin.Instance.JobsManager.Migrate(flags.ToArray());
        }

        public AllowedCaller AllowedCaller { get; } = AllowedCaller.Console;
        public string Name { get; } = "openmod";
        public string Help { get; } = "Installs OpenMod";
        public string Syntax { get; } = "<install>";
        public List<string> Aliases { get; } = new List<string>();
        public List<string> Permissions { get; } = new List<string>();
    }
}