namespace OpenMod.Installer.RocketMod.Jobs
{
    [Prevent("--no-extras")]
    public class OpenModEconomyInstallJob : NuGetInstallJob
    {
        public OpenModEconomyInstallJob() : base("OpenMod.Economy")
        {
        }
    }
}