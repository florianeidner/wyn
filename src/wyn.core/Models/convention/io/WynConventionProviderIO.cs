using Newtonsoft.Json;
using System.Collections.Generic;
using YamlDotNet.Serialization;

namespace wyn.core.Models
{
    public class WynConventionProviderIO
    {
        [YamlMember(Alias = "blocks")]
        [JsonProperty(PropertyName = "blocks")]
        public Dictionary<string, WynConventionNamingBlockIO> NamingBlocks { get; set; }

        [YamlMember(Alias = "structure")]
        [JsonProperty(PropertyName = "structure")]
        public string NamingStructure { get; set; }

        [YamlMember(Alias = "subs")]
        [JsonProperty(PropertyName = "subs")]
        public Dictionary<string, WynConventionProviderIO> SubConventionProviders { get; set; }

        [YamlMember(Alias = "tags")]
        [JsonProperty(PropertyName = "tags")]
        public Dictionary<string, string> Tags { get; set; }
    }
}
