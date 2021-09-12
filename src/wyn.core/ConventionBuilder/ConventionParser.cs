using Newtonsoft.Json;
using System;
using wyn.core.Models;
using YamlDotNet.Serialization;

namespace wyn.core.ConventionBuilder
{
    public class ConventionParser : IConventionBuilderStep
    {
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
                throw new NotImplementedException();
            }

            return convention;
        }

        internal static bool TryParseJson(string jsonString, out WynConventionIO result)
        {
            bool success = true;

            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<WynConventionIO>(jsonString, settings);
            return success;
        }

        internal static bool TryParseYaml(string yamlString, out WynConventionIO result)
        {
            var yamlDeserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();
            try
            {
                result = yamlDeserializer.Deserialize<WynConventionIO>(yamlString);
                return true;
            }
            catch
            {
                result = new WynConventionIO();
                return false;
            }
        }
    }
}
