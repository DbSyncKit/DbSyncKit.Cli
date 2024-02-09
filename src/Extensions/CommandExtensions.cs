using DbSyncKit.Cli.Commands;
using DbSyncKit.Cli.Commands.Configure.Create;
using DbSyncKit.DB.Enum;
using Spectre.Console.Cli;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static DbSyncKit.Cli.Commands.ConfigureCommand;

namespace DbSyncKit.Cli.Extensions
{
    public static class CommandExtensions
    {

        public static void AddInitCommand(this IConfigurator config)
        {
            config.AddCommand<InitCommand>("init").WithDescription("Initialize DbSyncKit");
        }

        public static void AddConfigCommand(this IConfigurator config)
        {
            config.AddBranch("config", command =>
            {
                command.SetDefaultCommand<ConfigureCommand>();//.WithDescription("Configure DbSyncKit");
                command.AddCommand<ConfigureCommand>("i").IsHidden().WithDescription("Configure DbSyncKit user interactive"); ;
                command.AddCommand<ListCommand>("list").WithDescription("Lists all provider configured");
                command.AddCreateBranchCommand();
            });
        }

        private static void AddCreateBranchCommand(this IConfigurator<CommandSettings> command)
        {
            command.AddBranch<CreateCommandSettings>("create", create =>
            {
                create.AddCommand<MSSQLCreateCommand>(nameof(DatabaseProvider.MSSQL)).WithDescription("Create MSSQL Settings");
            });
        }
    }
}
