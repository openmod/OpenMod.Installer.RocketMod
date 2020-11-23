using System;

namespace OpenMod.Installer.RocketMod.Jobs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PreventAttribute : Attribute
    {
        public PreventAttribute(string command)
        {
            CommandToPrevent = command;
        }

        public readonly string CommandToPrevent;
    }
}