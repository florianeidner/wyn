using Newtonsoft.Json;

namespace wyn.core.Models.terraform
{
    public class TfPlanResourceValues
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
