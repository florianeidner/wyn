using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using wyn.core.Enums;
using wyn.core.Models.terraform;

namespace wyn.core.Models
{
    public class WynConventionProvider : IWynConventionProvider
    {
        public Tuple<bool, List<string>> IsValid(WynConventionProvider parent)
        {
            var errors = new List<string>();

            if (this.NamingBlocks == null)
            {
                errors.Add("'namingblock' property missing");
            }

            // Check conventions
            if (!errors.Any()) errors.AddRange(this.CheckNamingStructure().Item2);

            // If conventions are fine, check naming blocks
            if (!errors.Any()) errors.AddRange(this.CheckNamingBlocks().Item2);

            return new Tuple<bool, List<string>>(!errors.Any(), errors);
        }

        public string Name { get; set; }

        public Dictionary<string, ConventionNamingBlock> NamingBlocks { get; set; }

        public WynConventionProvider Parent { get; set; }

        public List<WynConventionProvider> Children { get; set; }

        public string NamingStructure { get; set; }

        public Dictionary<string, string> Tags { get; set; }

        private Tuple<bool, List<string>> CheckNamingStructure()
        {
            var errors = new List<string>();
            if (String.IsNullOrWhiteSpace(this.NamingStructure))
                errors.Add($"{this.Name} naming structure is null or whitespace: '{this.NamingStructure}'");

            if (!errors.Any() && !CheckChars(this.NamingStructure))
            {
                errors.Add($"{this.Name} naming structure contains invalid chars: '{this.NamingStructure}'");
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
            MatchCollection matches = namingBlocks.Matches(this.NamingStructure);

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
            string name = NamingStructure;
            Regex namingBlocks = new Regex(@"\[[^\]\[]+\]");
            MatchCollection matches = namingBlocks.Matches(NamingStructure);

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

        public Tuple<bool, List<(ErrorType, string)>> CheckName(string name)
        {
            List<(ErrorType, string)> errors = new List<(ErrorType, string)>();

            // First split into namingblocks

            //find the delimiter:
            Regex nb = new Regex(@"\[[^\]\[]+\]");
            var namingBlocks = nb.Matches(this.NamingStructure);
            var d = nb.Split(this.NamingStructure);
            var delimiters = d.Skip(1).Take(d.Length - 2).ToArray();
            if (delimiters.Count() < (namingBlocks.Count() - 1) || delimiters.Any(d => d == ""))
            {
                errors.Add((ErrorType.warning, $"Cannot check '{name}' against convention: '{this.NamingStructure}'. Naming convention doesnt have delimiters between all blocks."));

            }
            else
            {
                var tempConvention = this.NamingStructure.ToString();
                var tempName = name.ToString();

                // Check each naming block for regex
                for (int i = 0; i < delimiters.Length; i++)
                {
                    var conventionSplit = tempConvention.Split(delimiters[i], 2);
                    tempConvention = conventionSplit[1];

                    var nameSplit = tempName.Split(delimiters[i], 2);

                    if (nameSplit.Length != 2)
                        errors.Add((ErrorType.error, $"{name}: Structure doesnt comply with convention {this.NamingStructure}"));

                    else
                    {
                        var nameBlock = nameSplit[0];
                        tempName = nameSplit[1];

                        var conventionNamingBlock = this.NamingBlocks.Where(n => n.Key == namingBlocks[i].Value.Substring(1, namingBlocks[i].Value.Length - 2)).Single();
                        if (!Regex.IsMatch(nameBlock, conventionNamingBlock.Value.Regex))
                            errors.Add((ErrorType.error, $"{name}: Name part '{nameBlock}' doesnt match with naming block '{conventionNamingBlock.Key}' regex {conventionNamingBlock.Value.Regex}"));
                    }
                }
            }

            return new Tuple<bool, List<(ErrorType, string)>>(!errors.Any(e => e.Item1 == ErrorType.error), errors);
        }

        public Tuple<bool, List<(ErrorType, string)>> CheckTfState(TfState state)
        {
            var errors = new List<(ErrorType, string)>();

            foreach (TfStateResource r in state.Resources)
            {
                foreach (TfInstance i in r.Instances)
                {
                    if (!String.IsNullOrWhiteSpace(i.Attributes.Name))
                    {

                        var providers = Children.Where(p =>
                        {
                            if (p.Tags != null && p.Tags.ContainsKey("tfResourceType") && p.Tags["tfResourceType"] == r.Type)
                            {
                                return true;
                            }
                            return false;
                        });

                        if (providers.Count() != 1)
                        {
                            errors.Add((ErrorType.warning, $"None or multiple providers found for terraform resource type: '{r.Type}'"));
                            continue;
                        }

                        var p = providers.Single();

                        errors.AddRange(p.CheckName(i.Attributes.Name).Item2);
                    }
                }
            }

            return new Tuple<bool, List<(ErrorType, string)>>(!errors.Any(e => e.Item1 == ErrorType.error), errors);
        }

        public Tuple<bool, List<(ErrorType, string)>> CheckTfPlan(TfPlan plan)
        {
            var errors = new List<(ErrorType, string)>();

            List<TfPlanResource> plannedRessources = plan.PlannedValues.RootModule.Resources;

            plan.PlannedValues.RootModule.ChildModules.ForEach(c => plannedRessources.AddRange(c.Resources));


            foreach (TfPlanResource r in plannedRessources)
            {
                if (!String.IsNullOrWhiteSpace(r.Values.Name))
                {

                    var providers = this.Children.Where(p =>
                    {
                        if (p.Tags != null && p.Tags.ContainsKey("tfResourceType") && p.Tags["tfResourceType"] == r.Type)
                        {
                            return true;
                        }
                        return false;
                    });

                    if (providers.Count() != 1)
                    {
                        errors.Add((ErrorType.warning, $"None or multiple providers found for terraform resource type: '{r.Type}'"));
                        continue;
                    }

                    var p = providers.Single();

                    errors.AddRange(p.CheckName(r.Values.Name).Item2);
                }
            }

            return new Tuple<bool, List<(ErrorType, string)>>(!errors.Any(e => e.Item1 == ErrorType.error), errors);
        }
    }
}
