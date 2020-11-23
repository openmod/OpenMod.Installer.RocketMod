namespace OpenMod.Installer.RocketMod.Jobs
{
    [Prevent("--no-extras")]
    public class OpenModCooldownsInstallJob : NuGetInstallJob
    {
        public OpenModCooldownsInstallJob() : base("OpenMod.Cooldowns")
        {
        }
    }
}