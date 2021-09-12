using Newtonsoft.Json;
using System.Collections.Generic;

namespace wyn.core.Models.terraform
{
    public class TfStateResource
    {
        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("provider")]
        public string Provider { get; set; }

        [JsonProperty("instances")]
        public List<TfInstance> Instances { get; set; }
    }
}
