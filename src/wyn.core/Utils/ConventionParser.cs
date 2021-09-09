using Newtonsoft.Json;
using YamlDotNet.Serialization;

namespace wyn.core.Utils
{
    public static class ConventionParser
    {
        public static bool TryParseConvention<T>(this string @this, out T result) where T : new()
        {
            if (TryParseJson<T>(@this, out T jsonResult))
            {
                result = jsonResult;
                return true;
            }
            else if (TryParseYaml<T>(@this, out T yamlResult))
            {
                result = yamlResult;
                return true;
            }
            else
            {
                result = new T();
                return false;
            }
        }

        internal static bool TryParseJson<T>(string jsonString, out T result)
        {
            bool success = true;

            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Error
            };
            result = JsonConvert.DeserializeObject<T>(jsonString, settings);
            return success;
        }

        internal static bool TryParseYaml<T>(string yamlString, out T result) where T : new()
        {
            var yamlDeserializer = new DeserializerBuilder()
                .IgnoreUnmatchedProperties()
                .Build();
            try
            {
                result = yamlDeserializer.Deserialize<T>(yamlString);
                return true;
            }
            catch
            {
                result = new T();
                return false;
            }
        }
    }
}
