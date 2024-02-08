using Spectre.Console.Cli;

namespace DbSyncKit.Cli.Commands
{
    public partial class ConfigureCommand
    {
        #region Sub commands

        //[Command("default", Description = "Set default configuration")]
        public class DefaultCommand : Command<Settings>
        {
            public override int Execute(CommandContext context, Settings settings)
            {
                // Implement logic to set the default configuration
                return 0;
            }
        }

        #endregion
    }
}
