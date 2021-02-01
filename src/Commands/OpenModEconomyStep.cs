using System;
using System.Collections.Generic;
using OpenMod.Installer.RocketMod.Jobs;

namespace OpenMod.Installer.RocketMod.Commands
{
    public class OpenModEconomyStep : CommandStep
    {
        public OpenModEconomyStep(List<IJob> jobs) : base(jobs)
        {
        }

        public override string Text { get; } = "Which economy system do you want to use?";

        public override List<CommandStepChoice> Choices { get; } = new List<CommandStepChoice>
        {
            new CommandStepChoice
            {
                Name = "RocketMod (recommended)",
                Description = "OpenMod will use RocketMod Uconomy for economy."
            },
            new CommandStepChoice
            {
                Name = "OpenMod",
                Description = "RocketMod will use OpenMod's economy system. Works with plugins that use Uconomy. Will import existing data from Uconomy."
            }
        };

        public override void OnChoice(string choice)
        {
            switch (choice.ToLowerInvariant())
            {
                case "rm":
                case "rocket":
                case "rocketmod":
                    return;
                case "om":
                case "openmod":
                    Jobs.Add(new OpenModEconomyInstallJob());
                    Jobs.Add(new MigrateEconomyJob());
                    return;
            }

            throw new InvalidChoiceException(choice);
        }
    }
}