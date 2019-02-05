namespace Catalog.API.Models
{
    public class EnvironmentConfiguration
    {
        public string ConnectionString { get; set; }
        public string EventBusConnection { get; set; }
        public string EventBusUsername { get; set; }
        public string EventBusPassword { get; set; }
        public int EventBusPort { get; set; }
    }
}
