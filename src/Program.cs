using DbSyncKit.Cli.Commands;
using Spectre.Console.Cli;

namespace DbSyncKit.Cli;

class Program
{
    static int Main(string[] args)
    {
        var app = new CommandApp();

        app.Configure(config =>
        {
            config.AddCommand<InitCommand>("init").WithDescription("Initialize DbSyncKit");
            //config.AddCommand<ConfigureCommand>("config").WithDescription("Configure DbSyncKit");
        });

        return app.Run(args);
    }
}
