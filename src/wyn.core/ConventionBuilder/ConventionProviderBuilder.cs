using System;
using System.Collections.Generic;
using System.Linq;
using wyn.core.Models;

namespace wyn.core
{
    public class ConventionProviderBuilder : IConventionBuilderStep
    {
        public WynConvention Build(WynConvention convention)
        {
            convention.Providers = new Dictionary<string, IWynConventionProvider>();

            var inputProviders = convention.ConventionInputObject.Providers;

            var listOfProviders = new List<WynConventionProvider>();
            foreach (var p in inputProviders)
            {
                listOfProviders.AddRange(BuildProviders(p.Value, p.Key, null));
            }

            listOfProviders.ForEach(p => convention.Providers.Add(p.Name, p));

            return convention;
        }

        private List<WynConventionProvider> BuildProviders(WynConventionProviderIO p, string name, WynConventionProvider parent)
        {          
            var provider = new WynConventionProvider();
            provider.Children = new List<WynConventionProvider>();
            provider.Parent = parent;

            // Name
            string namePrefix = "";
            if (parent != null)
                namePrefix = $"{parent.Name}.";
            provider.Name = $"{namePrefix}{name}";

            // Tags
            provider.Tags = p.Tags;


            // Naming structure
            if (parent != null)
                provider.NamingStructure = parent.NamingStructure;
            if (!String.IsNullOrWhiteSpace(p.NamingStructure))
                provider.NamingStructure = p.NamingStructure;

            // Naming blocks
            if (parent != null)
                provider.NamingBlocks = parent.NamingBlocks.ToDictionary(
                    entry => entry.Key,
                    entry => (ConventionNamingBlock)entry.Value.Clone());
            if (provider.NamingBlocks == null)
                provider.NamingBlocks = new Dictionary<string, ConventionNamingBlock>();

            if (p.NamingBlocks != null)
                foreach (var b in p.NamingBlocks)
                {
                    if (!provider.NamingBlocks.ContainsKey(b.Key))
                        provider.NamingBlocks.Add(b.Key, new ConventionNamingBlock()
                            {
                                Default = b.Value.Default,
                                Regex = b.Value.Regex
                            }
                        );
                    if (b.Value.Default != null)
                        provider.NamingBlocks[b.Key].Default = b.Value.Default;
                    if (b.Value.Regex != null)
                        provider.NamingBlocks[b.Key].Regex = b.Value.Regex;
                }

            // Add instance as child to parent
            if (parent != null)
                parent.Children.Add(provider);

            // Recurse through sub providers
            provider.Children = new List<WynConventionProvider>();

            var result = new List<WynConventionProvider>();
            if (p.SubConventionProviders != null)
                foreach(var s in p.SubConventionProviders)
                {
                    result.AddRange(BuildProviders(s.Value, s.Key, provider));
                }

            result.Add(provider);

            return result;
        }
    }
}
