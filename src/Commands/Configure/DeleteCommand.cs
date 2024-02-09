using Spectre.Console.Cli;

using static DbSyncKit.Cli.Commands.ConfigureCommand;

namespace DbSyncKit.Cli.Commands
{

    #region Sub commands

        //[Command("delete", Description = "Delete an existing configuration")]
        public class DeleteCommand : Command<ConfigureCommandSettings>
        {
            public override int Execute(CommandContext context, ConfigureCommandSettings settings)
            {
                // Implement logic to delete an existing configuration
                return 0;
            }
        }

        #endregion
    
}
