using Newtonsoft.Json;
using System;
using wyn.core.Models.terraform;

namespace wyn.core.Utils
{
    public static class TfPlanParser
    {
        public static bool TryParseTfPlan(this string @this, out TfPlan result)
        {
            bool success = true;

            var settings = new JsonSerializerSettings
            {
                Error = (sender, args) => { Console.WriteLine(args.ErrorContext.Error.Message); success = false; args.ErrorContext.Handled = true; },
                MissingMemberHandling = MissingMemberHandling.Ignore
            };
            result = JsonConvert.DeserializeObject<TfPlan>(@this, settings);
            return success;
        }
    }
}
