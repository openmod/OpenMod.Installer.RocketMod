using OpenMod.Installer.RocketMod.Jobs;
using Rocket.API;
using Rocket.Core.Logging;
using SDG.Unturned;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace OpenMod.Installer.RocketMod.Commands
{
    public class CommandOpenMod : IRocketCommand
    {
        private static readonly List<IJob> s_Jobs = new();
        private static readonly List<CommandWindowInputted> s_RocketModComandWindowsDelegates = new();
        private static CommandStep s_CurrentStep;
        private static string[] s_Args;
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

            s_Args = command;
            Logger.Log("Starting OpenMod installation..", ConsoleColor.DarkCyan);
            Logger.Log("Visit https://openmod.github.io/openmod-docs for information.", ConsoleColor.DarkCyan);
            Logger.Log("Type \"cancel\" to cancel.", ConsoleColor.Cyan);

            BindCommandInput();

            s_Jobs.Add(new OpenModModuleInstallJob());
            s_Jobs.Add(new OpenModAssemblyLoadJob());
            s_Jobs.Add(new OpenModUnturnedInstallJob());
            s_Jobs.Add(new PermissionsExInstallJob());

            s_CurrentStep = GetNextStep();
            PrintStep(s_CurrentStep);
        }
        private void BindCommandInput()
        {
            CommandWindow.onCommandWindowInputted += OnCommandWindowInputted;
            RemoveRocketModConsoleInput();
        }

        private void UnbindCommandInput()
        {
            CommandWindow.onCommandWindowInputted -= OnCommandWindowInputted;
            RestoreRocketModConsoleInput();
        }

        // ReSharper disable once RedundantAssignment
        private void OnCommandWindowInputted(string text, ref bool shouldExecuteCommand)
        {
            shouldExecuteCommand = false;
            text = text.Trim();

            if (text.Equals("cancel", StringComparison.OrdinalIgnoreCase))
            {
                Logger.Log("OpenMod installation aborted.", ConsoleColor.DarkRed);
                Reset();
                return;
            }

            try
            {
                Logger.Log($"Choice {text}");
                s_CurrentStep.OnChoice(text);
                s_CurrentStep = GetNextStep();

                if (s_CurrentStep == null)
                {
                    PerformMigration();
                }
                else
                {
                    PrintStep(s_CurrentStep);
                }
            }
            catch (InvalidChoiceException ex)
            {
                Logger.Log($"Invalid choice: {ex.Choice}. Type \"cancel\" to cancel OpenMod installation.", ConsoleColor.Red);
                PrintStep(s_CurrentStep);
            }
        }

        private void PrintStep(CommandStep step)
        {
            Logger.Log(string.Empty);
            Logger.Log(step.Text, ConsoleColor.DarkCyan);

            foreach (var choice in step.Choices)
            {
                Logger.Log($"  {choice.Name}: {choice.Description}", ConsoleColor.Cyan);
            }
        }

        private void PerformMigration()
        {
            try
            {
                JobsExecutor.Execute(s_Jobs, s_Args);
            }
            finally
            {
                Reset();
            }
        }

        private void RemoveRocketModConsoleInput()
        {
            if (s_RocketModComandWindowsDelegates.Count > 0)
            {
                return;
            }

            var commandWindowInputedInvocationList = CommandWindow.onCommandWindowInputted.GetInvocationList();
            foreach (var @delegate in commandWindowInputedInvocationList)
            {
                if (!IsRocketModDelegate(@delegate))
                {
                    continue;
                }

                var rocketModDelegate = (CommandWindowInputted)@delegate;
                CommandWindow.onCommandWindowInputted -= rocketModDelegate;
                s_RocketModComandWindowsDelegates.Add(rocketModDelegate);
            }
        }

        private void RestoreRocketModConsoleInput()
        {
            foreach (var rocketModDelegate in s_RocketModComandWindowsDelegates)
            {
                CommandWindow.onCommandWindowInputted += rocketModDelegate;
            }

            s_RocketModComandWindowsDelegates.Clear();
        }

        private bool IsRocketModDelegate(Delegate @delegate)
        {
            if (@delegate == null)
            {
                return false;
            }

            var methodInfo = @delegate.GetMethodInfo();
            var assembly = methodInfo?.DeclaringType?.Assembly;

            if (assembly == null)
            {
                return false;
            }

            var assemblyName = assembly.GetName();
            return assemblyName.Name.Equals("Rocket.Unturned")
                || assemblyName.Name.Equals("Rocket.Core");
        }

        private void Reset()
        {
            s_CurrentStep = null;
            s_Jobs.Clear();
            UnbindCommandInput();
            s_Args = null;
        }

        // Some really shitty state machine
        private CommandStep GetNextStep()
        {
            // null -> Permission -> Economy -> null

            return s_CurrentStep switch
            {
                null => new OpenModPermissionsStep(s_Jobs),
                OpenModPermissionsStep _ => new OpenModEconomyStep(s_Jobs),
                OpenModEconomyStep _ => null,
                // can not happen
                _ => throw new InvalidOperationException("Invalid state"),
            };
        }

        public AllowedCaller AllowedCaller { get; } = AllowedCaller.Console;
        public string Name { get; } = "openmod";
        public string Help { get; } = "Installs OpenMod";
        public string Syntax { get; } = "install";
        public List<string> Aliases { get; } = new List<string>();
        public List<string> Permissions { get; } = new List<string>();
    }
}