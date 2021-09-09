using System;
using System.Collections.Generic;

namespace wyn.core.Models
{
    public abstract class WynConventionBase : IWynConvention
    {
        public abstract Version GetVersion();

        public abstract Tuple<bool, List<string>> IsValid();
    }
}
