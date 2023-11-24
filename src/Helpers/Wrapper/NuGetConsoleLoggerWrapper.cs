using System.Reflection;

namespace OpenMod.Installer.RocketMod.Helpers.Wrapper
{
    public class NuGetConsoleLoggerWrapper : TypeWrapper
    {
        public NuGetConsoleLoggerWrapper(Assembly nugetAssembly) : base(nugetAssembly, "NuGetConsoleLogger")
        { }
    }
}