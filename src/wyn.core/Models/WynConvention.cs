using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace wyn.core.Models
{
    public class WynConvention : WynConventionBase
    {
        public override Version GetVersion()
        {
            return new Version(Version);
        }

        public override Tuple<bool, List<string>> IsValid()
        {
            var errors = new List<string>();

            if (Providers == null)
            {
                errors.Add("'providers' property missing.");
            }

            else
            {
                Providers.ToList().ForEach(p => errors.AddRange(p.Value.IsValid(null).Item2));
            }
            return new Tuple<bool, List<string>>(!errors.Any(), errors);
        }
        [YamlMember(Alias = "version")]
        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }

        [YamlMember(Alias = "providers")]
        [JsonProperty(PropertyName ="providers")]
        public Dictionary<string,WynConventionProvider> Providers { get; set; }
    }
}
