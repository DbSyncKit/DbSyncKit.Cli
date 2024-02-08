using Spectre.Console.Cli;

namespace DbSyncKit.Cli.Commands
{
    public partial class ConfigureCommand
    {

        #region Sub commands

        //[Command("delete", Description = "Delete an existing configuration")]
        public class DeleteCommand : Command<Settings>
        {
            public override int Execute(CommandContext context, Settings settings)
            {
                // Implement logic to delete an existing configuration
                return 0;
            }
        }

        #endregion
    }
}
