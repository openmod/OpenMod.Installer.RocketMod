using System;
using System.Collections.Generic;
using OpenMod.Installer.RocketMod.Jobs;

namespace OpenMod.Installer.RocketMod.Commands
{
    public class OpenModPermissionsStep : CommandStep
    {
        public OpenModPermissionsStep(List<IJob> jobs) : base(jobs)
        {
        }

        public override string Text { get; } = "Which permissions system do you want to use?";

        public override List<CommandStepChoice> Choices { get; } = new List<CommandStepChoice>
        {
            new CommandStepChoice
            {
                Name = "RocketMod (recommended)",
                Description = "OpenMod will use RocketMod's Permissions.config.xml for permissions."
            },
            new CommandStepChoice
            {
                Name = "OpenMod",
                Description = "RocketMod will use OpenMod's permission system. Will import existing permissions from RocketMod's Permissions.config.xml."
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
                    Jobs.Add(new MigratePermissionsJob());
                    return;
            }

            throw new InvalidChoiceException(choice);
        }
    }
}