using System;

namespace OpenMod.Installer.RocketMod.Jobs
{
    [AttributeUsage(AttributeTargets.Class)]
    public class PreventAttribute : Attribute
    {
        public PreventAttribute(string command)
        {
            _commandToPrevent = command;
        }

        public readonly string _commandToPrevent;
    }
}