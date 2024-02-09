using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Spectre.Console;
using Spectre.Console.Cli;
using DbSyncKit.Cli.Extensions;
using DbSyncKit.DB.Enum;
using DbSyncKit.Cli.Lang;
using DbSyncKit.Cli.Manager;

namespace DbSyncKit.Cli.Commands
{
    public partial class ConfigureCommand : Command<ConfigureCommand.ConfigureCommandSettings>
    {
        private readonly English _language;
        private readonly ConfigManager _configManager;

        public ConfigureCommand(English lang, ConfigManager config)
        {
            _language = lang;
            _configManager = config;
        }

        #region Settings

        public class ConfigureCommandSettings : CommandSettings
        {
        }

        #endregion

        #region Execution

        public override int Execute(CommandContext context, ConfigureCommandSettings settings)
        {
            if(context.Name != "i")
            {
                LoggerExtensions.Message("To use User interactive mode add i at the end\n", "yellow", "Hidden");
                LoggerExtensions.Message("DbSyncKit config i\n", "yellow", "Example");

                return -1;

            }
            

            if (!Directory.Exists(_configManager.DbSyncKitFolder))
            {
                LoggerExtensions.Error(_language.E_Initilized);
                return 0;
            }

            var configFile = _configManager.DbSyncKitConfig;
            var configurations = _configManager.configuration;
            Dictionary<string, Action> options = ConfigureOptionsInvokable(configFile, configurations);

            while (true)
            {
                AnsiConsole.Clear();

                _configManager.DisplayConfigurationsTable();

                string action = SelectConfigurationAction();

                if (options.TryGetValue(action, out var OptionsMethod))
                {
                    OptionsMethod.Invoke();
                }
                else
                    break;

            }

            options[_language.T_Exit].Invoke();
            return 0;
        }

        #endregion

        #region Configurations

        private Dictionary<string, Action> ConfigureOptionsInvokable(string configFile, DatabaseConfiguration configurations)
        {
            return new Dictionary<string, Action>
            {
                { _language.C_EditExistingConfig, () =>
                    {
                        var selectedConfig = SelectConfiguration(configurations.DatabaseConfigurations);
                        if (selectedConfig != null)
                        {
                            EditConfiguration(selectedConfig);
                            _configManager.SaveConfigurations(configurations, configFile);
                        }
                    }
                },
                { _language.C_NewConfig, () =>
                    {
                        AskQuestionsAndAddConfiguration(configurations, configFile);
                        _configManager.DisplayConfigurationsTable();
                    }
                },
                { _language.C_ChangeDefaultDB, () =>
                    {
                        var defaultConfig = SelectConfiguration(configurations.DatabaseConfigurations);
                        if (defaultConfig != null)
                        {
                            ChangeDefaultConfiguration(configurations, defaultConfig);
                            _configManager.SaveConfigurations(configurations, configFile);
                        }
                    }
                },
                { _language.C_ContractConfig, () =>
                    {
                        ContactRelatedConfigration(configurations);
                        _configManager.SaveConfigurations(configurations, configFile);
                    }
                },
                { _language.T_Exit, () =>
                    {
                        _configManager.SaveConfigurations(configurations, configFile);
                        Environment.Exit(0);
                    }
                }
            };
        }


        #endregion

        #region User Interaction

        private string SelectConfigurationAction()
        {
            var prompt = new SelectionPrompt<string>()
                .Title($"[yellow]{_language.Q_Select_Action}:[/]")
                .AddChoices(_language.C_EditExistingConfig, _language.C_NewConfig, _language.C_ChangeDefaultDB, _language.C_ContractConfig, _language.T_Exit);

            return AnsiConsole.Prompt(prompt);
        }

        private Configuration? SelectConfiguration(List<Configuration> configurations)
        {
            var prompt = new SelectionPrompt<string>()
                .Title($"[yellow]{_language.Q_Select_Config}:[/]")
                .PageSize(10);

            prompt.AddChoices(configurations.Select(config => config.Guid));
            prompt.AddChoices(_language.T_Back);

            var response = AnsiConsole.Prompt(prompt);

            return configurations.FirstOrDefault(config => config.Guid == response);
        }

        #endregion

        #region Configuration Editing

        private void EditConfiguration(Configuration config)
        {
            LoggerExtensions.Debug($"Editing configuration for {config.DatabaseProvider} - {config.Database}...");

            _configManager.DisplayConfigurationTable(config);


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
                    LoggerExtensions.Error(_language.E_PrividerIssue);
                    break;
            }


            LoggerExtensions.Success(_language.M_ConfigUpdated);
        }

        #endregion

        #region Configuration Addition

        private void AskQuestionsAndAddConfiguration(DatabaseConfiguration configurations, string configFile)
        {
            // Ask questions for the new configuration
            var newConfig = AskQuestions();

            configurations.DatabaseConfigurations.Add(newConfig);
            configurations.DefaultConfigurationGuid = newConfig.Guid;

            _configManager.SaveConfigurations(configurations, configFile);

            LoggerExtensions.Success(_language.M_ConfigAdded);
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
                    .Title($"[yellow]{_language.Q_Select_Provider}:[/]")
                    .PageSize(3)
                    .AddChoices(DatabaseProvider.MSSQL, DatabaseProvider.MySQL, DatabaseProvider.PostgreSQL)
            );
        }

        private void AskMSSQLQuestions(Configuration config)
        {
            config.UseServerAddress = PromptExtension.ConfirmWithDefault("[yellow]Do you want to use server address?[/]", config.UseServerAddress);

            if (config.UseServerAddress.GetValueOrDefault())
            {
                config.Server = PromptExtension.PromptWithDefault("[yellow]Enter MSSQL Server name or IP address:[/]", config.Server!);
                config.Database = PromptExtension.PromptWithDefault("[yellow]Enter MSSQL Database name:[/]", config.Database!);
            }
            else
            {
                config.DataSource = PromptExtension.PromptWithDefault("[yellow]Enter MSSQL Data source or IP address:[/]", config.DataSource!);
                config.InitialCatalog = PromptExtension.PromptWithDefault("[yellow]Enter MSSQL Initial catalog or Database name:[/]", config.InitialCatalog!);
            }

            config.IntegratedSecurity = PromptExtension.ConfirmWithDefault("[yellow]Use integrated security?[/]", config.IntegratedSecurity);

            if (!config.IntegratedSecurity.GetValueOrDefault())
            {
                config.UserID = PromptExtension.PromptWithDefault("[yellow]Enter MSSQL User ID:[/]", config.UserID!);

                config.Password = PromptExtension.PromptWithDefault("[yellow]Enter MSSQL Password:[/]", config.Password!);
            }
        }

        private void AskMySQLQuestions(Configuration config)
        {
            config.Server = PromptExtension.PromptWithDefault("[yellow]Enter MySQL Server name or IP address:[/]", config.Server!);
            config.Database = PromptExtension.PromptWithDefault("[yellow]Enter MySQL Database name:[/]", config.Database!);

            if (config.Port == default(int))
                config.Port = 3306;
            config.Port = PromptExtension.PromptWithDefault("[yellow]Enter MySQL Port number:[/]", config.Port);

            config.UserID = PromptExtension.PromptWithDefault("[yellow]Enter MySQL User:[/]", config.UserID!);

            config.Password = PromptExtension.PromptWithDefault("[yellow]Enter MySQL Password:[/]", config.Password!, true);

        }

        private void AskPostgreSQLQuestions(Configuration config)
        {
            config.Server = PromptExtension.PromptWithDefault("[yellow]Enter PostgreSQL Server name or IP address:[/]", config.Server!);
            config.Database = PromptExtension.PromptWithDefault("[yellow]Enter PostgreSQL Database name:[/]", config.Database!);

            if (config.Port == default(int))
                config.Port = 5432;

            config.Port = PromptExtension.PromptWithDefault("[yellow]Enter PostgreSQL Port number:[/]", config.Port);

            config.UserID = PromptExtension.PromptWithDefault("[yellow]Enter PostgreSQL User:[/]", config.UserID!);

            config.Password = PromptExtension.PromptWithDefault("[yellow]Enter PostgreSQL Password:[/]", config.Password!, true);
        }

        #endregion

        #region Configuration Default Change

        private void ChangeDefaultConfiguration(DatabaseConfiguration configurations, Configuration selectedConfig)
        {
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
    }
}
