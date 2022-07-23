namespace Play.Catalog.Service.Settings
{
    public class MongoDbSettings
    {
        public string Host { get; init; }
        public uint Port { get; init; } // init only assigns during initialize

        public string ConnectionString => $"mongodb://{Host}:{Port}";

    }
}