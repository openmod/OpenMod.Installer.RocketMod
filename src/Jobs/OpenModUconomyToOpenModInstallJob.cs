namespace OpenMod.Installer.RocketMod.Jobs
{
    [Prevent("--no-uconomy-link")]
    public class OpenModUconomyToOpenModInstallJob : NuGetInstallJob
    {
        public OpenModUconomyToOpenModInstallJob() : base("OpenMod.UconomyToOpenMod")
        {
        }
    }
}