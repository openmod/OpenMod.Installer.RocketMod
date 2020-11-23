namespace OpenMod.Installer.RocketMod.Jobs
{
    [Prevent("--no-extras")]
    public class OpenModPermissionsExInstallJob : NuGetInstallJob
    {
        public OpenModPermissionsExInstallJob() : base("DiFFoZ.PermissionExtensions")
        {
        }
    }
}