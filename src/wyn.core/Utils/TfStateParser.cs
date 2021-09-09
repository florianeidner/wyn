using Newtonsoft.Json;
using System;
using wyn.core.Models.terraform;

namespace wyn.core.Utils
{
    public static class TfStateParser
    {
        public static bool TryParseTfState(this string @this, out TfState result)
        {
            bool success = true;

            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { Console.WriteLine(args.ErrorContext.Error.Message); success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            result = JsonConvert.DeserializeObject<TfState>(@this, settings);
            return success;
        }
    }
}
