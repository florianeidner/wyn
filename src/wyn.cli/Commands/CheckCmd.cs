using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using wyn.core.Enums;

namespace wyn.cli.Commands
{
    [Command(Name = "check", OptionsComparison = System.StringComparison.InvariantCultureIgnoreCase)]
    class CheckCmd : WynCmdBase
    {
        public CheckCmd(ILogger<NameCmd> l, IConsole c)
        {
            _logger = l;
            _console = c;
        }

        protected override Task<int> OnExecute(CommandLineApplication app)
        {

            base.LoadConvention();
            base.LoadProvider();
            base.LoadTfState();

            var result = Provider.CheckTfState(TfStateObject);

            result.Item2.ForEach(e =>
            {
                switch (e.Item1)
                {
                    case ErrorType.error:
                        OutputError(e.Item2);
                        break;
                    case ErrorType.warning:
                        OutputWarning(e.Item2);
                        break;
                }
            });

            if (result.Item1 == true)
                OutputToConsole("TfState names compliant to naming convention.");

            return Task.FromResult(Convert.ToInt32(!result.Item1));
        }
    }
}
