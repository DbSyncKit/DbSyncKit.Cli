using DbSyncKit.Cli.Extensions;
using DbSyncKit.DB.Enum;
using Newtonsoft.Json;
using Spectre.Console;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DbSyncKit.Cli.Manager
{
    public class ConfigManager
    {
        public string DbSyncKitFolder => GetDBSyncKitFolder();

        public string DbSyncKitConfig => GetDBSyncKitConfig();

        public DatabaseConfiguration configuration => ReadConfigurations();


        public void DisplayConfigurationsTable()
        {
            var table = new Table();
            table.AddColumn("[yellow]Guid[/]");
            table.AddColumn("[yellow]Provider[/]");
            table.AddColumn("[yellow]Database Name[/]");

            foreach (var config in configuration.DatabaseConfigurations)
            {
                table.AddRow(config.Guid ?? string.Empty,config.DatabaseProvider.ToString(), config.Database ?? config.InitialCatalog ?? string.Empty);
            }

            AnsiConsole.Write(table);
        }

        public void DisplayConfigurationTable(Configuration config)
        {
            var table = new Table();
            table.AddColumn("[yellow]Property[/]");
            table.AddColumn("[yellow]Value[/]");

            table.AddRow("GUID", config.Guid);
            table.AddRow("DatabaseProvider", config.DatabaseProvider.ToString());

            if (config.UseServerAddress.GetValueOrDefault())
            {
                if (config.Server != null)
                    table.AddRow("Server", config.Server);

                if (config.Database != null)
                    table.AddRow("Database", config.Database);
            }
            else
            {
                if (config.DataSource != null)
                    table.AddRow("DataSource", config.DataSource);

                if (config.InitialCatalog != null)
                    table.AddRow("InitialCatalog", config.InitialCatalog);
            }

            if (config.DatabaseProvider == DatabaseProvider.MSSQL)
                table.AddRow("IntegratedSecurity", config.IntegratedSecurity.GetValueOrDefault().ToString());

            if (config.UserID != null)
                table.AddRow($"UserID", config.UserID);

            if (config.Password != null)
                table.AddRow("Password", config.Password);

            if (config.Port != 0)
                table.AddRow($"Port", config.Port.ToString());


            AnsiConsole.Write(table);
        }

        public void SaveConfigurations(DatabaseConfiguration dbConfig, string configFile)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dbConfig, Formatting.Indented);
                File.WriteAllText(configFile, json);
                LoggerExtensions.Success("Settings Saved successfully");
            }
            catch (Exception ex)
            {
                LoggerExtensions.Error($"Failed to save configuration file: {ex.Message}");
            }
        }

        private DatabaseConfiguration ReadConfigurations()
        {
            try
            {
                if (File.Exists(DbSyncKitConfig))
                {
                    var json = File.ReadAllText(DbSyncKitConfig);
                    return JsonConvert.DeserializeObject<DatabaseConfiguration>(json);
                }
            }
            catch (Exception ex)
            {
                LoggerExtensions.Error($"Failed to read configuration file: {ex.Message}");
            }

            return new DatabaseConfiguration();
        }

        private string GetDBSyncKitConfig()
        {
            return Path.Combine(DbSyncKitFolder, "config.json");
        }

        private string GetDBSyncKitFolder()
        {
            return Path.Combine(Environment.CurrentDirectory, ".DbSyncKit");
        }
    }
}
