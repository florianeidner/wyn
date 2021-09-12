using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using wyn.core.Models;
using YamlDotNet.Serialization;

namespace wyn.core
{
    public class ConventionParser : IConventionBuilderStep
    {
        private List<string> errorList;

        public ConventionParser()
        {
            errorList = new();
        }

        public WynConvention Build(WynConvention convention) 
        {
            if (TryParseJson(convention.ConventionInputString, out WynConventionIO jsonResult))
            {
                convention.ConventionInputObject = jsonResult;
            }
            else if (TryParseYaml(convention.ConventionInputString, out WynConventionIO yamlResult))
            {
                convention.ConventionInputObject = yamlResult;
            }
            else
            {
                throw new ConventionBuilderException(errorList);
            }

            return convention;
        }

        internal bool TryParseJson(string jsonString, out WynConventionIO result)
        {
            bool success = true;

            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => {
                    success = false;
                    errorList.Add($"JSON parser error: {args.ErrorContext.Error.Message}");
                    args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            result = JsonConvert.DeserializeObject<WynConventionIO>(jsonString, settings);
            return success;
        }

        internal bool TryParseYaml(string yamlString, out WynConventionIO result)
        {
            var yamlDeserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();
            try
            {
                result = yamlDeserializer.Deserialize<WynConventionIO>(yamlString);
                return true;
            }
            catch(Exception ex)
            {
                errorList.Add($"YAML parser error: {ex.Message}");
                result = new();
                return false;
            }
        }
    }
}
