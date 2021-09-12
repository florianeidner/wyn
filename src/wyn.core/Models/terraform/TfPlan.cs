using Newtonsoft.Json;

namespace wyn.core.Models.terraform
{
    public class TfPlan
    {
        [JsonProperty("format_version")]
        public string FormatVersion { get; set; }

        [JsonProperty("terraform_version")]
        public string TerraformVersion { get; set; }

        [JsonProperty("variables")]
        public dynamic Variables { get; set; }

        [JsonProperty("planned_values")]
        public TfPlannedValues PlannedValues { get; set; }

        //[JsonProperty("resource_changes")]
        //public List<ResourceChanx> ResourceChanges { get; set; }

        //[JsonProperty("prior_state")]
        //public PriorState PriorState { get; set; }

        //[JsonProperty("configuration")]
        //public Configuration Configuration { get; set; }
    }
}
