using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using YamlDotNet.Serialization;

namespace wyn.core.Models
{
    public class ConventionNamingBlock : IConventionNamingBlock
    {
        public Tuple<bool, List<string>> IsValid()
        {
            var errors = new List<string>();
            try
            {
                bool match = System.Text.RegularExpressions.Regex.IsMatch(Default, Regex);
                if (!match)
                    errors.Add($"Default value: '{Default}' not matching the regex: '{Regex}'");
            }
            catch (RegexParseException ex)
            {
                errors.Add($"'{Regex}' is not a valid regular expression: {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                errors.Add($"Properties 'default', and 'regex' cannot be null: {ex.Message}");
            }
            return new Tuple<bool, List<string>>(!errors.Any(), errors);
        }

        [YamlMember(Alias = "default")]
        [JsonProperty(PropertyName = "default")]
        public string Default { get; set; }

        [YamlMember(Alias = "regex")]
        [JsonProperty(PropertyName = "regex")]
        public string Regex { get; set; }
    }
}
