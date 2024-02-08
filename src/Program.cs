using DbSyncKit.Cli.Commands;
using DbSyncKit.Cli.Extensions;
using DbSyncKit.Cli.Infrastructure;
using DbSyncKit.Cli.Lang;
using DbSyncKit.Cli.Manager;
using Microsoft.Extensions.DependencyInjection;
using Spectre.Console.Cli;

namespace DbSyncKit.Cli;

class Program
{
    static int Main(string[] args)
    {
        var services = new ServiceCollection();
        services.AddScoped<English>();
        services.AddSingleton<ConfigManager>();

        var registrar = new TypeRegistrar(services);

        var app = new CommandApp(registrar);

        app.Configure(config =>
        {
            config.SetApplicationName("DbSyncKit");
            config.AddInitCommand();
            config.AddConfigCommand();
            config.AddCommand<StatusCommand>("status").WithDescription("status");
            //config.AddCommand<ConfigureCommand>("config").WithDescription("Configure DbSyncKit");
        });

        return app.Run(args);
    }
}
