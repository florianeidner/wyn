using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using wyn.cli.Enums;
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
            base.LoadTfFile();

            Tuple<bool, List<(ResultType, string)>> result = null;

            if (TfFileObject.Item1 == TfFileType.tfplan)
                result = Provider.CheckTfPlan(TfFileObject.Item2);
            else if (TfFileObject.Item1 == TfFileType.tfstate)
                result = Provider.CheckTfState(TfFileObject.Item2);
            else
                OutputError("TfFile couldnt be read", true);
            
            result.Item2.Distinct().ToList().OrderBy(s => s.Item1).ToList().ForEach(e =>
            {
                switch (e.Item1)
                {
                    case ResultType.success:
                        OutputSuccess(e.Item2);
                        break;
                    case ResultType.error:
                        OutputError(e.Item2);
                        break;
                    case ResultType.warning:
                        OutputWarning(e.Item2);
                        break;
                }
            });

            if (result.Item1 == true)
                OutputToConsole("Terraform names compliant to naming convention.");

            return Task.FromResult(Convert.ToInt32(!result.Item1));
        }
    }
}
