using System.Text.RegularExpressions;

namespace OpenMod.Installer.RocketMod.Jobs
{
    public class OpenModUnturnedConfigEconomyJob : OpenModUnturnedConfigJob
    {
        private readonly string m_DesiredEconomySystem;

        public OpenModUnturnedConfigEconomyJob(string desiredEconomySystem)
        {
            m_DesiredEconomySystem = desiredEconomySystem;
        }

        protected override string ConfigType => "Economy System";

        protected override void ModifyConfig(ref string configText)
        {
            // Regex: ^  economySystem:.*$
            configText = Regex.Replace(
                configText,
                @"^  economySystem:.*$",
                "  economySystem: " + m_DesiredEconomySystem);
        }
    }
}
