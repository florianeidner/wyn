using System;
using System.Collections.Generic;

namespace wyn.core.Models
{
    public interface IWynConventionProvider
    {
        public Tuple<bool, List<string>> IsValid(WynConventionProvider parent);
        string Convention { get; set; }
        Dictionary<string, ConventionNamingBlock> NamingBlocks { get; set; }
        Dictionary<string, WynConventionProvider> SubConventionProviders { get; set; }
    }
}