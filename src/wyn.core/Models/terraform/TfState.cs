using Newtonsoft.Json;
using System.Collections.Generic;

namespace wyn.core.Models.terraform
{
    public class TfState
    {
        [JsonProperty("version")]
        public int Version { get; set; }

        [JsonProperty("terraform_version")]
        public string TerraformVersion { get; set; }

        [JsonProperty("serial")]
        public int Serial { get; set; }

        [JsonProperty("lineage")]
        public string Lineage { get; set; }

        [JsonProperty("outputs")]
        public object Outputs { get; set; }

        [JsonProperty("resources")]
        public List<TfResource> Resources { get; set; }
    }
}
