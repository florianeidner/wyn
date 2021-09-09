using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace wyn.cli.Commands
{
    [Command(Name = "wyn", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    [Subcommand(
        typeof(NameCmd),
        typeof(ConventionCmd),
        typeof(CheckCmd)
        )]

    class WynCmd : WynCmdBase
    {
        public WynCmd(ILogger<WynCmd> l, IConsole c)
        {
            _logger = l;
            _console = c;
        }
        protected override Task<int> OnExecute(CommandLineApplication app)
        {
            // this shows help even if the --help option isn't specified
            app.ShowHelp();
            return Task.FromResult(0);
        }
    }
}
