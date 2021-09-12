using System;
using System.Collections.Generic;
using wyn.core.Enums;
using wyn.core.Models.terraform;

namespace wyn.core.Models
{
    public interface IWynConventionProvider
    {
        public Tuple<bool, List<string>> IsValid(WynConventionProvider parent);

        public string GenerateName(Dictionary<string, string> values);

        public Tuple<bool, List<(ResultType, string)>> CheckTfState(TfState state);

        public Tuple<bool, List<(ResultType, string)>> CheckTfPlan(TfPlan plan);

    }
}