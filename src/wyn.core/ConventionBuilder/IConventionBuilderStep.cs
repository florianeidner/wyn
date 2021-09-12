using wyn.core.Models;

namespace wyn.core
{
    public interface IConventionBuilderStep
    {
        public WynConvention Build(WynConvention convention);
    }
}
