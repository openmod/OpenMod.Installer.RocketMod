using System.Text.RegularExpressions;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class OpenModUnturnedConfigPermissionsJob : OpenModUnturnedConfigJob
    {
        private readonly string m_DesiredPermissionSystem;

        public OpenModUnturnedConfigPermissionsJob(string desiredPermissionSystem)
        {
            m_DesiredPermissionSystem = desiredPermissionSystem;
        }

        protected override string ConfigType => "Permissions System";

        protected override void ModifyConfig(ref string configText)
        {
            // Regex: ^  permissionSystem:.*$
            configText = Regex.Replace(
                configText,
                @"^  permissionSystem:.*$",
                "  permissionSystem: " + m_DesiredPermissionSystem);
        }
    }
}
