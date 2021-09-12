using Newtonsoft.Json;
using System.Collections.Generic;

namespace wyn.core.Models.terraform
{
    public class TfPlanResource
    {
        [JsonProperty("address")]
        public string Address { get; set; }

        [JsonProperty("mode")]
        public string Mode { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("provider_name")]
        public string ProviderName { get; set; }

        [JsonProperty("schema_version")]
        public int SchemaVersion { get; set; }

        [JsonProperty("values")]
        public TfPlanResourceValues Values { get; set; }

        //[JsonProperty("sensitive_values")]
        //public SensitiveValues SensitiveValues { get; set; }

        //[JsonProperty("index")]
        //public object Index { get; set; }

        //[JsonProperty("provider_config_key")]
        //public string ProviderConfigKey { get; set; }

        //[JsonProperty("expressions")]
        //public Expressions Expressions { get; set; }

        //[JsonProperty("depends_on")]
        //public List<string> DependsOn { get; set; }

        //[JsonProperty("for_each_expression")]
        //public ForEachExpression ForEachExpression { get; set; }

        //[JsonProperty("count_expression")]
        //public CountExpression CountExpression { get; set; }
    }
}
