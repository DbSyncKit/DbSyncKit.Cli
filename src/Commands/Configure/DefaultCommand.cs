using Spectre.Console.Cli;

using static DbSyncKit.Cli.Commands.ConfigureCommand;

namespace DbSyncKit.Cli.Commands
{

    #region Sub commands

        //[Command("default", Description = "Set default configuration")]
        public class DefaultCommand : Command<ConfigureCommandSettings>
        {
            public override int Execute(CommandContext context, ConfigureCommandSettings settings)
            {
                // Implement logic to set the default configuration
                return 0;
            }
        }

        #endregion
    
}
