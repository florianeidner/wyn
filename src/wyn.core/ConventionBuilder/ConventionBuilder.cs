using System.Collections.Generic;
using wyn.core.Models;

namespace wyn.core
{
    public class ConventionBuilder
    {
        internal readonly List<IConventionBuilderStep> BuildSteps;

        public ConventionBuilder()
        {
            BuildSteps = new List<IConventionBuilderStep>();
            BuildSteps.Add(new ConventionParser());
            BuildSteps.Add(new ConventionProviderBuilder());
        }

        internal ConventionBuilder(List<IConventionBuilderStep> buildSteps)
        {
            BuildSteps = buildSteps;
        }

        public IWynConvention CreateFromConventionString(string conventionString)
        {
            var pipeline = BuildSteps.GetEnumerator();

            WynConvention convention = new(conventionString);

            while(pipeline.MoveNext())
            {
                convention = pipeline.Current.Build(convention);
            }
            return convention;
        }
    }
}
