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
                Value = "OpenMod will use RocketMod Uconomy for economy."
            },
            new CommandStepChoice
            {
                Name = "OpenMod",
                Value = "RocketMod will use OpenMod's economy system. Works with plugins that use Uconomy. Will import existing data from Uconomy."
            }
        };

        public override void OnChoice(string choice)
        {
            switch (choice.ToLowerInvariant())
            {
                case "RocketMod":
                    break;
                case "OpenMod":
                    Jobs.Add(new OpenModEconomyInstallJob());
                    Jobs.Add(new MigrateEconomyJob());
                    break;
            }

            throw new InvalidOperationException();
        }
    }
}