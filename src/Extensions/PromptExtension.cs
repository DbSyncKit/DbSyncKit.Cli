using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbSyncKit.Cli.Extensions
{
    public class PromptExtension
    {
        public static T PromptWithDefault<T>(string promptText, T defaultValue, bool isPassword = false) where T : notnull
        {
            var prompt = new TextPrompt<T>(promptText);
            if (!EqualityComparer<T>.Default.Equals(defaultValue, default))
                prompt.DefaultValue(defaultValue);

            if (isPassword)
            {
                prompt.Secret();
                prompt.PromptStyle("red");
                prompt.AllowEmpty();
            }


            return AnsiConsole.Prompt(prompt);
        }

        public static bool ConfirmWithDefault(string promptText, bool? defaultValue)
        {
            var prompt = new ConfirmationPrompt(promptText);

            if (defaultValue.HasValue)
                prompt.DefaultValue = defaultValue.Value;

            return AnsiConsole.Prompt(prompt);
        }
    }
}
