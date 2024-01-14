using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Cli;
using DbSyncKit.Cli.Extensions;
using DbSyncKit.DB.Enum;

namespace DbSyncKit.Cli.Commands
{
    public class ConfigureCommand : Command<ConfigureCommand.Settings>
    {
        #region Settings

        public class Settings : CommandSettings { }

        #endregion

        #region Execution

        public override int Execute(CommandContext context, Settings settings)
        {
            var dbSyncKitFolder = Path.Combine(Environment.CurrentDirectory, ".DbSyncKit");
            if (!Directory.Exists(dbSyncKitFolder))
            {
                LoggerExtensions.Error("DbSyncKit is not initialized!");
                return 0;
            }

            var configFile = Path.Combine(dbSyncKitFolder, "config.json");
            var configurations = ReadConfigurations(configFile);


            while (true)
            {
                AnsiConsole.Clear();
                DisplayConfigurationsTable(configurations.DatabaseConfigurations);

                var action = SelectConfigurationAction();

                switch (action)
                {
                    case "Edit Existing Configuration":
                        var selectedConfig = SelectConfiguration(configurations.DatabaseConfigurations);
                        if (selectedConfig != null)
                        {
                            EditConfiguration(selectedConfig);
                            SaveConfigurations(configurations, configFile);

                        }
                        break;

                    case "Add New Configuration":
                        AskQuestionsAndAddConfiguration(configurations, configFile);
                        DisplayConfigurationsTable(configurations.DatabaseConfigurations); // Display updated list
                        break;

                    case "Change Default Database":
                        var defaultConfig = SelectConfiguration(configurations.DatabaseConfigurations);
                        if (defaultConfig != null)
                        {
                            ChangeDefaultConfiguration(configurations, defaultConfig);
                            SaveConfigurations(configurations, configFile);
                        }
                        break;
                    case "Configure Data Contract related Configuration":
                        ContactRelatedConfigration(configurations);
                        SaveConfigurations(configurations, configFile);
                        break;

                    case "Exit":
                        // User chose to exit
                        SaveConfigurations(configurations, configFile);
                        return 0;
                }
            }
        }

        #endregion

        #region Configuration Management

        private DatabaseConfiguration ReadConfigurations(string configFile)
        {
            try
            {
                if (File.Exists(configFile))
                {
                    var json = File.ReadAllText(configFile);
                    return JsonConvert.DeserializeObject<DatabaseConfiguration>(json);
                }
            }
            catch (Exception ex)
            {
                LoggerExtensions.Error($"Failed to read configuration file: {ex.Message}");
            }

            return new DatabaseConfiguration();
        }

        private void SaveConfigurations(DatabaseConfiguration dbConfig, string configFile)
        {
            try
            {
                var json = JsonConvert.SerializeObject(dbConfig, Formatting.Indented);
                File.WriteAllText(configFile, json);
            }
            catch (Exception ex)
            {
                LoggerExtensions.Error($"Failed to save configuration file: {ex.Message}");
            }
        }

        #endregion

        #region User Interaction

        private string SelectConfigurationAction()
        {
            var prompt = new SelectionPrompt<string>()
                .Title("[yellow]Select an Action:[/]")
                .AddChoices("Edit Existing Configuration", "Add New Configuration", "Change Default Database", "Configure Data Contract related Configuration", "Exit");

            return AnsiConsole.Prompt(prompt);
        }

        private Configuration SelectConfiguration(List<Configuration> configurations)
        {
            var prompt = new SelectionPrompt<Configuration>()
                .Title("[yellow]Select a Configuration:[/]")
                .PageSize(10);

            prompt.AddChoices(configurations);

            return AnsiConsole.Prompt(prompt);
        }

        private void DisplayConfigurationsTable(List<Configuration> configurations)
        {
            var table = new Table();
            table.AddColumn("[yellow]Provider[/]");
            table.AddColumn("[yellow]Database Name[/]");

            foreach (var config in configurations)
            {
                table.AddRow(config.DatabaseProvider.ToString(), config.Database ?? config.InitialCatalog!);
            }

            AnsiConsole.Write(table);
        }

        #endregion

        #region Configuration Editing

        private void EditConfiguration(Configuration config)
        {
            LoggerExtensions.Debug($"Editing configuration for {config.DatabaseProvider} - {config.Database}...");

            DisplayConfigurationTable(config);


                //config.DatabaseProvider;
            switch (config.DatabaseProvider)
            {
                case DatabaseProvider.MSSQL:
                    AskMSSQLQuestions(config);
                    break;
                case DatabaseProvider.MySQL:
                    AskMySQLQuestions(config);
                    break;
                case DatabaseProvider.PostgreSQL:
                    AskPostgreSQLQuestions(config);
                    break;
                default:
                    LoggerExtensions.Error("Provider Does not exists");
                    break;
            }
            

            LoggerExtensions.Success("Configuration updated successfully!");
        }

        private void DisplayConfigurationTable(Configuration config)
        {
            var table = new Table();
            table.AddColumn("[yellow]Property[/]");
            table.AddColumn("[yellow]Value[/]");

            table.AddRow("GUID",config.Guid);
            table.AddRow("DatabaseProvider", config.DatabaseProvider.ToString());

            if (config.UseServerAddress.GetValueOrDefault())
            {
                if(config.Server != null)
                    table.AddRow("Server", config.Server);

                if(config.Database != null)
                    table.AddRow("Database", config.Database);
            }
            else
            {
                if (config.DataSource != null)
                    table.AddRow("DataSource", config.DataSource);

                if (config.InitialCatalog != null)
                    table.AddRow("InitialCatalog", config.InitialCatalog);
            }

            if(config.DatabaseProvider == DatabaseProvider.MSSQL)
                table.AddRow("IntegratedSecurity", config.IntegratedSecurity.GetValueOrDefault().ToString());

            if (config.UserID != null)
                table.AddRow($"UserID", config.UserID);

            if (config.Password != null)
                table.AddRow("Password", config.Password);

            if (config.Port != 0)
                table.AddRow($"Port", config.Port.ToString());


            AnsiConsole.Write(table);
        }

        private string SelectPropertyToEdit()
        {
            var prompt = new SelectionPrompt<string>()
                .Title("[yellow]Select a Property to Edit:[/]")
                .PageSize(5)
                .AddChoices("Provider", "UseServerAddress", "Server", /* Add other properties... */ "Exit");

            return AnsiConsole.Prompt(prompt);
        }

        #endregion

        #region Configuration Addition

        private void AskQuestionsAndAddConfiguration(DatabaseConfiguration configurations, string configFile)
        {
            // Ask questions for the new configuration
            var newConfig = AskQuestions();

            configurations.DatabaseConfigurations.Add(newConfig);
            configurations.DefaultConfigurationGuid = newConfig.Guid;

            SaveConfigurations(configurations, configFile);

            LoggerExtensions.Success("New configuration added successfully!");
        }

        private Configuration AskQuestions()
        {
            var config = new Configuration();
            config.Guid = Guid.NewGuid().ToString();
            // Ask for general database configuration
            config.DatabaseProvider = AskDatabaseProvider();

            // Ask type-specific questions
            switch (config.DatabaseProvider)
            {
                case DatabaseProvider.MSSQL:
                    AskMSSQLQuestions(config);
                    break;

                case DatabaseProvider.MySQL:
                    AskMySQLQuestions(config);
                    break;

                case DatabaseProvider.PostgreSQL:
                    AskPostgreSQLQuestions(config);
                    break;
            }

            return config;
        }

        private DatabaseProvider AskDatabaseProvider()
        {
            return AnsiConsole.Prompt(
                new SelectionPrompt<DatabaseProvider>()
                    .Title("[yellow]Choose a Database Provider:[/]")
                    .PageSize(3)
                    .AddChoices(DatabaseProvider.MSSQL, DatabaseProvider.MySQL, DatabaseProvider.PostgreSQL)
            );
        }

        private void AskMSSQLQuestions(Configuration config)
        {
            config.UseServerAddress = ConfirmWithDefault("[yellow]Do you want to use server address?[/]", config.UseServerAddress);

            if (config.UseServerAddress.GetValueOrDefault())
            {
                config.Server = PromptWithDefault("[yellow]Enter MSSQL Server name or IP address:[/]", config.Server!);
                config.Database = PromptWithDefault("[yellow]Enter MSSQL Database name:[/]", config.Database!);
            }
            else
            {                
                config.DataSource = PromptWithDefault("[yellow]Enter MSSQL Data source or IP address:[/]", config.DataSource!);
                config.InitialCatalog = PromptWithDefault("[yellow]Enter MSSQL Initial catalog or Database name:[/]", config.InitialCatalog!);
            }

            config.IntegratedSecurity = ConfirmWithDefault("[yellow]Use integrated security?[/]", config.IntegratedSecurity);

            if (!config.IntegratedSecurity.GetValueOrDefault())
            {
                config.UserID = PromptWithDefault("[yellow]Enter MSSQL User ID:[/]", config.UserID!);

                config.Password = PromptWithDefault("[yellow]Enter MSSQL Password:[/]", config.Password!);
            }
        }

        private void AskMySQLQuestions(Configuration config)
        {
            config.Server = PromptWithDefault("[yellow]Enter MySQL Server name or IP address:[/]", config.Server!);
            config.Database = PromptWithDefault("[yellow]Enter MySQL Database name:[/]", config.Database!);

            if (config.Port == default(int))
                config.Port = 3306;
            config.Port = PromptWithDefault("[yellow]Enter MySQL Port number:[/]", config.Port);

            config.UserID = PromptWithDefault("[yellow]Enter MySQL User:[/]", config.UserID!);

            config.Password = PromptWithDefault("[yellow]Enter MySQL Password:[/]", config.Password!,true);

        }

        private void AskPostgreSQLQuestions(Configuration config)
        {
            config.Server = PromptWithDefault("[yellow]Enter PostgreSQL Server name or IP address:[/]", config.Server!);
            config.Database = PromptWithDefault("[yellow]Enter PostgreSQL Database name:[/]", config.Database!);

            if (config.Port == default(int))
                config.Port = 5432;

            config.Port = PromptWithDefault("[yellow]Enter PostgreSQL Port number:[/]", config.Port);

            config.UserID = PromptWithDefault("[yellow]Enter PostgreSQL User:[/]", config.UserID!);

            config.Password = PromptWithDefault("[yellow]Enter PostgreSQL Password:[/]",config.Password!,true);
        }

        #endregion

        #region Configuration Default Change

        private void ChangeDefaultConfiguration(DatabaseConfiguration configurations, Configuration selectedConfig)
        {
            // Implement logic to set the selected configuration as default
            configurations.DefaultConfigurationGuid = selectedConfig.Guid;
            LoggerExtensions.Success($"Default configuration changed to {selectedConfig.DatabaseProvider} - {selectedConfig.Database}");
        }

        #endregion

        #region Configuration Data Contract Related

        //TODO: Configure DataContract Related Default Configuration
        private void ContactRelatedConfigration(DatabaseConfiguration configurations)
        {

        }

        #endregion

        #region Helper
        private T PromptWithDefault<T>(string promptText, T defaultValue, bool isPassword = false) where T : notnull
        {
            var prompt = new TextPrompt<T>(promptText);
            if (!EqualityComparer<T>.Default.Equals(defaultValue, default))
                prompt.DefaultValue(defaultValue);

            if (isPassword)
            {
                prompt.Secret();
                prompt.PromptStyle("red");
                prompt.AllowEmpty();
            }


            return AnsiConsole.Prompt(prompt);
        }

        private bool ConfirmWithDefault(string promptText, bool? defaultValue)
        {
            var prompt = new ConfirmationPrompt(promptText);

            if (defaultValue.HasValue)
                prompt.DefaultValue = defaultValue.Value;

            return AnsiConsole.Prompt(prompt);
        }

        #endregion

        #region Configuration Classes

        private class DatabaseConfiguration
        {
            public List<Configuration> DatabaseConfigurations { get; set; } = new List<Configuration>();
            public string? DefaultConfigurationGuid { get; set; }

            public string? DefaultDataContractPath { get; set; }
        }

        private class Configuration
        {
            public string Guid { get; set; }
            public DatabaseProvider DatabaseProvider { get; set; }
            public bool? UseServerAddress { get; set; }
            public string? DataSource { get; set; }
            public string? Server { get; set; }
            public string? Database { get; set; }
            public string? InitialCatalog { get; set; }
            public string? UserID { get; set; }
            public string? Password { get; set; }
            public int Port { get; set; }
            public bool? IntegratedSecurity { get; set; }

            public override string ToString()
            {
                // Customize the string representation for each configuration
                return $"{DatabaseProvider} - {Database ?? InitialCatalog}";
            }
        }

        #endregion
    }
}
