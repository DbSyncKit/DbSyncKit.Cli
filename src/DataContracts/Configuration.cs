using DbSyncKit.DB.Enum;

namespace DbSyncKit.Cli.Manager
{
    public class Configuration
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
            return $"{DatabaseProvider} - {Database ?? InitialCatalog} ({Guid})";
        }
    }
    
}
