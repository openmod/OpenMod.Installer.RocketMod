namespace OpenMod.Installer.RocketMod.Jobs
{
    [Prevent("--no-permission-link")]
    public class OpenModPermissionLinkInstallJob : NuGetInstallJob
    {
        public OpenModPermissionLinkInstallJob() : base("OpenMod.Rocket.PermissionLink")
        {
        }
    }
}