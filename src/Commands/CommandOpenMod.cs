using Rocket.API;
using Rocket.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using OpenMod.Installer.RocketMod.Jobs;
using SDG.Unturned;

namespace OpenMod.Installer.RocketMod.Commands
{
    public class CommandOpenMod : IRocketCommand
    {
        private static readonly List<IJob> s_Jobs = new List<IJob>();
        private static CommandStep s_CurrentStep;

        public void Execute(IRocketPlayer caller, string[] command)
        {
            if (command.Length < 1)
            {
                Logger.Log("Invalid command usage. Use \"openmod install\".");
                return;
            }

            var force = command.Any(d => d.Trim().Equals("--force") || d.Trim().Equals("-f"));
            if (!force && OpenModInstallerPlugin.Instance.OpenModManager.IsOpenModInstalled)
            {
                Logger.LogWarning("OpenMod is already installed, aborting. Use \"openmod install -f\" to install anyway.");
                return;
            }

            if (!force && Provider.clients.Count > 0)
            {
                Logger.LogWarning("Players are online. Installing OpenMod while players are online may disconnect them. Use \"openmod install -f\" to install anyway.");
                return;
            }

            Reset();
            Logger.Log("Starting OpenMod installation.");
            Logger.Log("Type \"cancel\" to cancel installation.");

            BindCommandInput();

            s_Jobs.Add(new OpenModModuleInstallJob());
            s_Jobs.Add(new OpenModAssemblyLoadJob());
            s_Jobs.Add(new PermissionsExInstallJob());

            s_CurrentStep = GetNextStep();
            PrintStep(s_CurrentStep);
        }
        private void BindCommandInput()
        {
            CommandWindow.onCommandWindowInputted += OnCommandWindowInputted;
        }

        private void UnbindCommandInput()
        {
            CommandWindow.onCommandWindowInputted -= OnCommandWindowInputted;
        }

        private void OnCommandWindowInputted(string text, ref bool shouldexecutecommand)
        {
            text = text.Trim();

            if (text.Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                Logger.Log("Aborting OpenMod installation.");
                Reset();
                return;
            }

            try
            {
                s_CurrentStep.OnChoice(text);
                s_CurrentStep = GetNextStep();

                if (s_CurrentStep == null)
                {
                    PerformMigration();
                }
            }
            catch (InvalidOperationException)
            {
                Logger.Log($"Invalid choice: {text}.");
                PrintStep(s_CurrentStep);
            }
        }

        private void PerformMigration()
        {
            JobsExecutor.Execute(s_Jobs);
            Reset();
        }

        private void PrintStep(CommandStep step)
        {
            Logger.Log(string.Empty);
            Logger.Log(step.Text);

            foreach (var choice in step.Choices)
            {
                Logger.Log($"  {choice.Name}: {choice.Description}");
            }
        }

        private void Reset()
        {
            s_CurrentStep = null;
            s_Jobs.Clear();
            UnbindCommandInput();
        }

        // Some really shitty state machine
        private CommandStep GetNextStep()
        {
            // null -> Permission -> Economy -> null

            switch (s_CurrentStep)
            {
                case null:
                    return new OpenModPermissionsStep(s_Jobs);
                case OpenModPermissionsStep _:
                    return new OpenModEconomyStep(s_Jobs);
                case OpenModEconomyStep _:
                    return null;
            }

            // can not happen
            throw new InvalidOperationException("Invalid state");
        }

        public AllowedCaller AllowedCaller { get; } = AllowedCaller.Console;
        public string Name { get; } = "openmod";
        public string Help { get; } = "Installs OpenMod";
        public string Syntax { get; } = "install";
        public List<string> Aliases { get; } = new List<string>();
        public List<string> Permissions { get; } = new List<string>();
    }
}