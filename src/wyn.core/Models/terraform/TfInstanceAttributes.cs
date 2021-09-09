using Newtonsoft.Json;

namespace wyn.core.Models.terraform
{
    public class TfInstanceAttributes
    {
        [JsonProperty("name")]
        public string Name { get; set; }
    }
}
