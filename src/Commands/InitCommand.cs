using DbSyncKit.DB.Enum;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DbSyncKit.Cli.Extensions;

namespace DbSyncKit.Cli.Commands
{
    public class InitCommand : Command<InitCommand.Settings>
    {
        public class Settings : CommandSettings { }

        public override int Execute(CommandContext context, Settings settings)
        {
            AnsiConsole.Status()
                .Start("Initializing DbSyncKit...", ctx =>
                {
                    LoggerExtensions.Log("Initializing DbSyncKit...");
                    CreateOrClearDirectory(".DbSyncKit");

                    // Update the status
                    ctx.Status("Initialization complete");
                    ctx.Spinner(Spinner.Known.Star);
                    ctx.SpinnerStyle(Style.Parse("green"));

                    // Finalize the status
                    ctx.Status("DbSyncKit initialized successfully!");
                    LoggerExtensions.Success("DbSyncKit initialized successfully!");

                });

            return 0;
        }

        private void CreateOrClearDirectory(string directoryPath)
        {
            if (Directory.Exists(directoryPath))
            {
                LoggerExtensions.Warn($"Deleting existing {directoryPath} at {Path.GetFullPath(directoryPath)}...");
                Directory.Delete(directoryPath, true);
            }

            LoggerExtensions.Log("Creating .DbSyncKit folder...");

            Directory.CreateDirectory(directoryPath);

            var fullPath = Path.GetFullPath(directoryPath);

            LoggerExtensions.Success($"Created {directoryPath} at {fullPath}...");
        }
    }
}
