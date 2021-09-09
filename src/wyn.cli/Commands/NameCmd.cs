using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace wyn.cli.Commands
{
    [Command(Name = "name", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    class NameCmd : WynCmdBase
    {
        public NameCmd(ILogger<NameCmd> l, IConsole c)
        {
            _logger = l;
            _console = c;
        }

        protected override Task<int> OnExecute(CommandLineApplication app)
        {

            base.LoadConvention();
            base.LoadProvider();
            base.LoadAdditionalParameters();

            OutputToConsole(Provider.GenerateName(AdditionalParameters));

            return Task.FromResult(0);
        }
    }
}
