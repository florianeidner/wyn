using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YamlDotNet.Serialization;

namespace wyn.core.Models
{
    public class WynConvention : IWynConvention
    {

        internal WynConvention(string conventionString)
        {
            ConventionInputString = conventionString;
        }
        internal string ConventionInputString { get; set; }

        internal WynConventionIO ConventionInputObject { get; set; }

        public string Version { get; set; }

        internal Dictionary<string, IWynConventionProvider> Providers { get; set; }

        public Tuple<bool, List<string>> IsValid()
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

        public Version GetVersion()
        {
            return new Version(Version);
        }

        public IEnumerable<IWynConventionProvider> GetProviders()
        {
            return Providers.Values;
        }

        public IWynConventionProvider GetProvider(string name)
        {
            return Providers[name];
        }
    }
}
