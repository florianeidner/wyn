using System;
using System.Collections.Generic;

namespace wyn.core.Models
{
    public interface IWynConvention
    {
        public Version GetVersion();
        public Tuple<bool, List<string>> IsValid();
        public IEnumerable<IWynConventionProvider> GetProviders();
        public IWynConventionProvider GetProvider(string name);
    }
}