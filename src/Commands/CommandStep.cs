using System.Collections.Generic;
using OpenMod.Installer.RocketMod.Jobs;

namespace OpenMod.Installer.RocketMod.Commands
{
    public abstract class CommandStep
    {
        protected List<IJob> Jobs { get; }

        protected CommandStep(List<IJob> jobs)
        {
            Jobs = jobs;
        }

        public abstract string Text { get; }

        public abstract List<CommandStepChoice> Choices { get; }

        public abstract void OnChoice(string choice);
    }
}