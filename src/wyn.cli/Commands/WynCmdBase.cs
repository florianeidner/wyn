using McMaster.Extensions.CommandLineUtils;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using wyn.cli.Enums;
using wyn.core.Models;
using wyn.core.Models.terraform;
using wyn.core.Utils;

namespace wyn.cli.Commands
{
    [HelpOption("--help")]
    abstract class WynCmdBase
    {
        [Option(CommandOptionType.SingleValue, ShortName = "c", LongName = "convention", Description = "Convention as JSON string", ValueName = "convention from json", ShowInHelpText = true)]
        private string ConventionJson { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "u", LongName = "convention-url", Description = "URL to convention", ValueName = "convention from url", ShowInHelpText = true)]
        private string ConventionUrl { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "f", LongName = "convention-file", Description = "path to yaml or json for convention", ValueName = "convention from file", ShowInHelpText = true)]
        private string ConventionFilePath { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "p", LongName = "provider", Description = "Provider for naming convention", ValueName = "provider", ShowInHelpText = true)]
        private string SelectedProviderInput { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "a", LongName = "additional-params", Description = "Additional parameters, comma separated list. eg name=fix,noname=4", ValueName = "additional params", ShowInHelpText = true)]
        private string AdditionalParametersInput { get; set; }

        [Option(CommandOptionType.SingleValue, ShortName = "t", LongName = "tfstate-file", Description = "Terraform statefile to check for naming convention", ValueName = "tfstate file", ShowInHelpText = true)]
        private string TfStateFilePath { get; set; }

        protected ILogger _logger;
        protected IConsole _console;
        internal WynConvention Convention { get; set; }

        internal WynConventionProvider Provider { get; set; }

        internal TfState TfStateObject { get; set; }

        internal Dictionary<string, string> AdditionalParameters { get; set; }

        protected virtual Task<int> OnExecute(CommandLineApplication app)
        {
            return Task.FromResult(0);
        }

        protected void LoadConvention()
        {
            string conventionString = "";
            Tuple<ConventionSource, string> conventionSource;
            string envVar = Environment.GetEnvironmentVariable(Constants.ENV_CONVENTION);

            //Select which source to load convention from
            if (!String.IsNullOrWhiteSpace(ConventionJson))
                conventionSource = new Tuple<ConventionSource, string>(ConventionSource.inline, ConventionJson);

            else if (!String.IsNullOrWhiteSpace(ConventionFilePath))
                conventionSource = new Tuple<ConventionSource, string>(ConventionSource.file,ConventionFilePath);

            else if (!String.IsNullOrWhiteSpace(ConventionUrl))
                conventionSource = new Tuple<ConventionSource, string>(ConventionSource.url, ConventionUrl);

            else if (!String.IsNullOrWhiteSpace(envVar) && envVar.Split("=", 2).Length == 2)
            {
                string[] envVarArray = envVar.Split("=", 2);
                conventionSource = new Tuple<ConventionSource, string>(Enum.Parse<ConventionSource>(envVarArray[0]), envVarArray[1]);

            }
            else
                conventionSource = new Tuple<ConventionSource, string>(ConventionSource.none, null);

            switch (conventionSource.Item1)
            {
                case ConventionSource.inline:
                    conventionString = conventionSource.Item2;
                    break;

                case ConventionSource.file:
                    using (StreamReader reader = new StreamReader(conventionSource.Item2))
                    {
                        conventionString = reader.ReadToEnd();
                    }
                    break;
                
                case ConventionSource.url:
                    throw new NotImplementedException();
                
                case ConventionSource.git:
                    throw new NotImplementedException();
                
                default:
                    OutputError("Please provide a convention.", true);
                    break;
            }

            if (!conventionString.TryParseConvention(out WynConvention c))
                OutputError("Convention input is not a valid json or yaml.");

            var r = c.IsValid();
            if (r.Item1 == false)
            {
                string errorMessage = "Convention is invalid:";
                r.Item2.ForEach(m => errorMessage += $"\n\t{m}");
                OutputError(errorMessage, true);
            }

            Convention = c;
        }

        protected void LoadProvider()
        {
            string p = "";
            if (!String.IsNullOrWhiteSpace(SelectedProviderInput))
                p = SelectedProviderInput;

            else if (!String.IsNullOrWhiteSpace(Environment.GetEnvironmentVariable(Constants.ENV_PROVIDER)))
                p = Environment.GetEnvironmentVariable(Constants.ENV_PROVIDER);
            else 
                OutputError("Please set a provider.", true);

            if (Convention == null) OutputError("Convention not loaded", true);

            string[] providerPath = p.Split(".");
            if (!Convention.Providers.TryGetValue(providerPath[0], out WynConventionProvider provider))
                OutputError($"Provider '{providerPath[0]}' not found.", true);

            //Select subproviders
            for (int i = 1; i < providerPath.Length; i++)
            {
                if (provider.SubConventionProviders.TryGetValue(providerPath[i], out WynConventionProvider subprovider))
                {
                    provider = subprovider.CopyFromParent(provider);
                }
                else
                {
                    OutputError($"Subprovider '{providerPath[i]}' not found");
                    break;
                }
            }
            Provider = provider;
        }

        protected void LoadTfState()
        {
            string tfStateString = "";

            using (StreamReader reader = new StreamReader(TfStateFilePath))
            {
                tfStateString = reader.ReadToEnd();
            }

            if (tfStateString.TryParseTfState(out TfState s)) {
                TfStateObject = s;
            }

            else
            {
                OutputError("Error parsing tf state file.", false);
            }

        }

        protected void LoadAdditionalParameters()
        {
            AdditionalParameters = new Dictionary<string, string>();
            try
            {
                if (AdditionalParametersInput == null) return;
                var parameters = AdditionalParametersInput.Split(",");
                foreach (string p in parameters)
                {
                    var kvp = p.Split("=");
                    AdditionalParameters.Add(kvp[0], kvp[1]);
                }
            }
            catch
            {
                OutputError("Error reading Additional Paramters.", true);
            }
        }

        protected void OutputToConsole(string data)
        {
            _console.BackgroundColor = ConsoleColor.Black;
            _console.ForegroundColor = ConsoleColor.White;
            _console.Out.Write(data);
            _console.ResetColor();
        }

        protected void OutputError(string message, bool throwException = false)
        {
            _console.ForegroundColor = ConsoleColor.Red;
            _console.Error.WriteLine(message);
            _console.ResetColor();
            if (throwException) throw new Exception(message);
        }

        protected void OutputWarning(string message)
        {
            _console.ForegroundColor = ConsoleColor.Yellow;
            _console.Error.WriteLine(message);
            _console.ResetColor();
        }
    }
}
