using Newtonsoft.Json;
using System.Collections.Generic;

namespace wyn.core.Models.terraform
{
    public class TfInstance
    {
        [JsonProperty("schema_version")]
        public int SchemaVersion { get; set; }

        [JsonProperty("attributes")]
        public TfInstanceAttributes Attributes { get; set; }

        [JsonProperty("sensitive_attributes")]
        public List<object> SensitiveAttributes { get; set; }

        [JsonProperty("private")]
        public string Private { get; set; }

        [JsonProperty("dependencies")]
        public List<string> Dependencies { get; set; }

        [JsonProperty("index_key")]
        public object IndexKey { get; set; }
    }
}
