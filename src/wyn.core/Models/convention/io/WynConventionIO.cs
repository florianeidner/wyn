using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace wyn.core.Models
{
    public class WynConventionIO
    {
        [YamlMember(Alias = "version")]
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [YamlMember(Alias = "providers")]
        [JsonProperty(PropertyName = "providers")]
        public Dictionary<string, WynConventionProviderIO> Providers { get; set; }

        [YamlMember(Alias = "name")]
        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [YamlMember(Alias = "schema")]
        [JsonProperty(PropertyName = "schema")]
        public string Schema { get; set; }
    }
}
