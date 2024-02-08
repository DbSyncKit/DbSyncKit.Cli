using Spectre.Console.Cli;
using DbSyncKit.Cli.Manager;

namespace DbSyncKit.Cli.Commands
{
    public partial class ConfigureCommand
    {
        #region Sub commands

        public class ListCommand : Command
        {
            private readonly ConfigManager _configManager;

            public ListCommand(ConfigManager configManager)
            {
                _configManager = configManager;
            }

            public override int Execute(CommandContext context)
            {
                _configManager.DisplayConfigurationsTable();
                return 0;
            }
        }

        #endregion
    }
}
