namespace DbSyncKit.Cli.Manager
{
    public class DatabaseConfiguration
    {
        public List<Configuration> DatabaseConfigurations { get; set; } = new List<Configuration>();
        public string? DefaultConfigurationGuid { get; set; }

        public string? DefaultDataContractPath { get; set; }
    }
    
}
