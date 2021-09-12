using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

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

        public object Clone()
        {
            return new ConventionNamingBlock()
            {
                Default = this.Default.ToString(),
                Regex = this.Regex.ToString()
            };
        }

        public string Default { get; set; }

        public string Regex { get; set; }
    }
}
