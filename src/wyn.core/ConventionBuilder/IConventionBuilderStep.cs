using wyn.core.Models;

namespace wyn.core.ConventionBuilder
{
    public interface IConventionBuilderStep
    {
        public WynConvention Build(WynConvention convention);
    }
}
