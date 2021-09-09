using System;
using System.Collections.Generic;

namespace wyn.core.Models
{
    public interface IConventionNamingBlock
    {
        string Default { get; set; }
        string Regex { get; set; }

        Tuple<bool, List<string>> IsValid();
    }
}