using Spectre.Console.Cli;
using DbSyncKit.Cli.Extensions;
using DbSyncKit.DB.Enum;
using System.ComponentModel;

namespace DbSyncKit.Cli.Commands
{
    public class CreateCommandSettings : CommandSettings
    {
        [CommandOption("-i|--interactive")]
        public bool Interactive { get; set; }

    }
}
