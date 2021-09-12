using System;
using System.Collections.Generic;

namespace wyn.core
{
    public class ConventionBuilderException : Exception
    {
        public ConventionBuilderException(List<string> messages) : base(CreateMessage(messages)) { }

        private static string CreateMessage(List<string> messages)
        {
            string message = "Error parsing wyn convention:";
            messages.ForEach(m => message += $"\n{m}");
            return message;
        }
    }
}