using Newtonsoft.Json;

namespace OpenMod.Installer.RocketMod.Models
{
    public class Asset
    {
        [JsonProperty("browser_download_url")]
        public string BrowserDownloadUrl { get; set; }
    }
}
