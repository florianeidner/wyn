using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace wyn.core.Models
{
    public class WynConventionNamingBlockIO
    {
        [YamlMember(Alias = "default")]
        [JsonProperty(PropertyName = "default")]
        public string Default { get; set; }

        [YamlMember(Alias = "regex")]
        [JsonProperty(PropertyName = "regex")]
        public string Regex { get; set; }
    }
}
