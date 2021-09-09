using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using wyn.core.Enums;
using wyn.core.Models.terraform;
using YamlDotNet.Serialization;

namespace wyn.core.Models
{
    public class WynConventionProvider : IWynConventionProvider
    {
        public Tuple<bool, List<string>> IsValid(WynConventionProvider parent)
        {
            var errors = new List<string>();

            var tempProvider = CopyFromParent(parent);

            if (tempProvider.NamingBlocks == null)
            {
                errors.Add("'namingblock' property missing");
            }

            // Check conventions
            if (!errors.Any()) errors.AddRange(tempProvider.CheckConvention().Item2);

            // If conventions are fine, check naming blocks
            if (!errors.Any()) errors.AddRange(tempProvider.CheckNamingBlocks().Item2);


            // Check subs
            if (!errors.Any() && tempProvider.SubConventionProviders != null)
            {
                foreach (var p in tempProvider.SubConventionProviders)
                {

                    errors.AddRange(p.Value.IsValid(tempProvider).Item2);
                }
            }
            return new Tuple<bool, List<string>>(!errors.Any(), errors);
        }

        [YamlMember(Alias = "namingBlocks")]
        [JsonProperty(PropertyName = "namingBlocks")]
        public Dictionary<string, ConventionNamingBlock> NamingBlocks { get; set; }

        [YamlMember(Alias = "convention")]
        [JsonProperty(PropertyName = "convention")]
        public string Convention { get; set; }

        [YamlMember(Alias = "subs")]
        [JsonProperty(PropertyName = "subs")]
        public Dictionary<string, WynConventionProvider> SubConventionProviders { get; set; }

        [YamlMember(Alias = "tags")]
        [JsonProperty(PropertyName = "tags")]
        public Dictionary<string, string> Tags { get; set; }

        private Tuple<bool, List<string>> CheckConvention()
        {
            var errors = new List<string>();
            if (String.IsNullOrWhiteSpace(Convention))
                errors.Add($"Convention is null or whitespace: '{Convention}'");

            if (!errors.Any() && !CheckChars(Convention))
            {
                errors.Add($"Convention contains invalid chars: '{Convention}'");
            }
            return new Tuple<bool, List<string>>(!errors.Any(), errors);
        }

        private Tuple<bool, List<string>> CheckNamingBlocks()
        {
            var errors = new List<string>();
            //Check Naming blocks consistency
            foreach (var b in NamingBlocks)
            {
                errors.AddRange(b.Value.IsValid().Item2);
            }

            //Check if naming block has all required fields used in convention
            Regex namingBlocks = new Regex(@"\[[^\]\[]+\]");
            MatchCollection matches = namingBlocks.Matches(Convention);

            foreach (Match m in matches)
            {
                var s = m.ToString();
                s = s.Substring(1, s.Length - 2);
                if (!NamingBlocks.ContainsKey(s))
                {
                    errors.Add($"Convention contains missing namingBlock: '{m}'");
                }
            }

            return new Tuple<bool, List<string>>(!errors.Any(), errors);
        }

        private bool CheckChars(string s)
        {
            Regex invalidChars = new Regex(@"[^-a-zA-Z\d\[\]]");
            if (invalidChars.IsMatch(s)) return false;
            return true;
        }

        public string GenerateName(Dictionary<string, string> values)
        {
            string name = Convention;
            Regex namingBlocks = new Regex(@"\[[^\]\[]+\]");
            MatchCollection matches = namingBlocks.Matches(Convention);

            foreach (Match m in matches)
            {
                string match = m.ToString();
                string key = match.Substring(1, m.Length - 2);

                if (values != null && values.TryGetValue(key, out string value))
                {
                    var nb = new ConventionNamingBlock()
                    {
                        Regex = NamingBlocks[key].Regex,
                        Default = value
                    };
                    var validation = nb.IsValid();
                    if (validation.Item1 == false) throw new Exception($"Input not according convention: '{nb.Default}' not matchin '{nb.Regex}'");
                    name = name.Replace(match, value);
                }
                else
                {
                    name = name.Replace(match, NamingBlocks[key].Default);
                }
            }
            return name;
        }

        public WynConventionProvider CopyFromParent(WynConventionProvider parent)
        {
            if (parent == null) return this;

            WynConventionProvider tempProvider = (WynConventionProvider)parent.MemberwiseClone();
            tempProvider.SubConventionProviders = this.SubConventionProviders;


            if (!String.IsNullOrWhiteSpace(Convention)) { tempProvider.Convention = Convention; }

            if (NamingBlocks != null)
            {
                foreach (var b in NamingBlocks)
                {
                    if (!tempProvider.NamingBlocks.ContainsKey(b.Key)) tempProvider.NamingBlocks.Add(b.Key, b.Value);
                    if (b.Value.Default != null) tempProvider.NamingBlocks[b.Key].Default = b.Value.Default;
                    if (b.Value.Regex != null) tempProvider.NamingBlocks[b.Key].Regex = b.Value.Regex;
                }
            }

            return tempProvider;
        }

        public Tuple<bool, List<(ErrorType,string)>> CheckName(string name)
        {
            List<(ErrorType,string)> errors = new List<(ErrorType,string)>();

            // First split into namingblocks

            //find the delimiter:
            Regex nb = new Regex(@"\[[^\]\[]+\]");
            var namingBlocks = nb.Matches(this.Convention);
            var d = nb.Split(this.Convention);
            var delimiters = d.Skip(1).Take(d.Length - 2).ToArray();
            if (delimiters.Count() < (namingBlocks.Count() - 1) || delimiters.Any(d => d == ""))
            {
                errors.Add((ErrorType.warning,$"Naming convention doesnt have delimiters between all blocks. Cannot check '{name}' against convention: '{this.Convention}'"));

            }
            else {
                var tempConvention = Convention.ToString();
                var tempName = name.ToString();

                // Check each naming block for regex
                for (int i = 0; i < delimiters.Length; i++)
                {
                    var conventionSplit = tempConvention.Split(delimiters[i], 2);
                    tempConvention = conventionSplit[1];
                    
                    var nameSplit = tempName.Split(delimiters[i], 2);
                    var nameBlock = nameSplit[0];
                    tempName = nameSplit[1];

                    var conventionNamingBlock = this.NamingBlocks.Where(n => n.Key == namingBlocks[i].Value.Substring(1, namingBlocks[i].Value.Length - 2)).Single();
                    if (!Regex.IsMatch(nameBlock, conventionNamingBlock.Value.Regex))
                        errors.Add((ErrorType.error,$"{name}: Name part '{nameBlock}' doesnt match with naming block '{conventionNamingBlock.Key}' regex {conventionNamingBlock.Value.Regex}"));
                }
            }

            return new Tuple<bool, List<(ErrorType,string)>>(!errors.Any(e => e.Item1 == ErrorType.error),errors);
        }

        public Tuple<bool, List<(ErrorType,string)>> CheckTfState(TfState state)
        {
            var errors = new List<(ErrorType,string)>();

            foreach (TfResource r in state.Resources)
            {
                foreach (TfInstance i in r.Instances)
                {
                    if (!String.IsNullOrWhiteSpace(i.Attributes.Name)) {

                        var providers = SubConventionProviders.Where(p =>
                        {
                            if (p.Value.Tags != null && p.Value.Tags.ContainsKey("tfResourceType") && p.Value.Tags["tfResourceType"] == r.Type)
                            {
                                return true;
                            }
                            return false;
                        });

                        if (providers.Count() != 1)
                        {
                            errors.Add((ErrorType.warning,$"None or multiple providers found for terraform resource type: '{r.Type}'"));
                            continue;
                        }

                        var p = providers.Single().Value;
                        p = p.CopyFromParent(this);
                        errors.AddRange(p.CheckName(i.Attributes.Name).Item2);  
                    }
                }
            }

            return new Tuple<bool, List<(ErrorType, string)>>(!errors.Any(e => e.Item1 == ErrorType.error), errors);
        }
    }
}
