using System;

namespace OpenMod.Installer.RocketMod.Commands
{
    public sealed class InvalidChoiceException : Exception
    {
        public string Choice { get; }

        public InvalidChoiceException(string choice)
        {
            Choice = choice;
        }
    }
}