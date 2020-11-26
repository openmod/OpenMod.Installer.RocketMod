using Newtonsoft.Json;
using System.Collections.Generic;

namespace OpenMod.Installer.RocketMod.Models
{
    public class LatestRelease
    {
        [JsonProperty("assets")]
        public List<Asset> Assets { get; set; }
    }
}
