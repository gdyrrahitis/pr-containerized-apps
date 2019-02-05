namespace Orders.API.Models
{
    public class EnvironmentConfiguration
    {
        public string ConnectionString { get; set; }
        public string DatabaseName { get; set; }
        public string CollectionName { get; set; }
        public string EventBusConnection { get; set; }
        public int EventBusPort { get; set; }
    }
}
