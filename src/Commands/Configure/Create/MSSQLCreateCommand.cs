using DbSyncKit.Cli.Extensions;
using DbSyncKit.Cli.Manager;
using Spectre.Console;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbSyncKit.Cli.Commands.Configure.Create
{
    public class MSSQLCreateCommand : Command<MSSQLCreateCommand.Settings>
    {
        public class Settings : ConfigureCommand.CreateCommandSettings
        {
            [CommandOption("-s|--server")]
            [Description("Server name or IP address")]
            public string Server { get; set; }

            [CommandOption("-d|--database")]
            [Description("Database name")]
            public string Database { get; set; }

            [CommandOption("--port")]
            [Description("Port number for the Database (optional)")]
            public int? Port { get; set; }

            [CommandOption("-i|--integrated-security")]
            [Description("Use Windows Integrated security Default false")]
            [DefaultValue(false)]
            public bool IntegratedSecurity { get; set; }

            [CommandOption("-u|--user")]
            [Description("Username for the database")]
            public string? Username { get; set; }

            [CommandOption("-p|--password")]
            [Description("Password for the database")]
            public string? Password { get; set; }


        }

        public override int Execute(CommandContext context, Settings settings)
        {


            return 0;
        }

        public override ValidationResult Validate(CommandContext context, Settings settings)
        {
            if (!settings.IntegratedSecurity && string.IsNullOrEmpty(settings.Username) || string.IsNullOrEmpty(settings.Password))
            {
                return ValidationResult.Error("User & password is required when integrated security is false");
            }

            return base.Validate(context, settings);
        }

        private void AskMSSQLQuestions(Configuration config)
        {
            config.UseServerAddress = PromptExtension.ConfirmWithDefault("[yellow]Do you want to use server address?[/]", config.UseServerAddress);

            if (config.UseServerAddress.GetValueOrDefault())
            {
                config.Server = PromptExtension.PromptWithDefault("[yellow]Enter MSSQL Server name or IP address:[/]", config.Server!);
                config.Database = PromptExtension.PromptWithDefault("[yellow]Enter MSSQL Database name:[/]", config.Database!);
            }
            else
            {
                config.DataSource = PromptExtension.PromptWithDefault("[yellow]Enter MSSQL Data source or IP address:[/]", config.DataSource!);
                config.InitialCatalog = PromptExtension.PromptWithDefault("[yellow]Enter MSSQL Initial catalog or Database name:[/]", config.InitialCatalog!);
            }

            config.IntegratedSecurity = PromptExtension.ConfirmWithDefault("[yellow]Use integrated security?[/]", config.IntegratedSecurity);

            if (!config.IntegratedSecurity.GetValueOrDefault())
            {
                config.UserID = PromptExtension.PromptWithDefault("[yellow]Enter MSSQL User ID:[/]", config.UserID!);

                config.Password = PromptExtension.PromptWithDefault("[yellow]Enter MSSQL Password:[/]", config.Password!);
            }
        }

        private bool? ConfirmWithDefault(string v, bool? useServerAddress)
        {
            throw new NotImplementedException();
        }
    }
}
