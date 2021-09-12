using Newtonsoft.Json;
using System.Collections.Generic;

namespace wyn.core.Models.terraform
{
    public class TfPlannedValues
    {
        [JsonProperty("root_module")]
        public TfRootModule RootModule { get; set; }
    }

    public class TfRootModule
    {
        [JsonProperty("resources")]
        public List<TfPlanResource> Resources { get; set; }

        [JsonProperty("child_modules")]
        public List<TfChildModule> ChildModules { get; set; }

        //[JsonProperty("module_calls")]
        //public dynamic ModuleCalls { get; set; }

        //[JsonProperty("variables")]
        //public dynamic Variables { get; set; }
    }

    public class TfChildModule
    {
        [JsonProperty("resources")]
        public List<TfPlanResource> Resources { get; set; }

        //[JsonProperty("address")]
        //public string Address { get; set; }
    }
}
