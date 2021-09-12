using System.Collections.Generic;
using wyn.core.Models;

namespace wyn.core.ConventionBuilder
{
    public class ConventionBuilder
    {
        List<IConventionBuilderStep> BuildSteps;

        public ConventionBuilder()
        {
            BuildSteps = new List<IConventionBuilderStep>();
        }

        public IWynConvention CreateFromString(string conventionString)
        {
            BuildSteps.Add(new ConventionParser());
            BuildSteps.Add(new ConventionProviderBuilder());

            var pipeline = BuildSteps.GetEnumerator();

            WynConvention convention = new WynConvention(conventionString);

            while(pipeline.MoveNext())
            {
                convention = pipeline.Current.Build(convention);
            }
            return convention;
        }
    }
}
