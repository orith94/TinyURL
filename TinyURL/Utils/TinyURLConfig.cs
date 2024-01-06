namespace TinyURL.Utils
{
    public class TinyURLConfig
    {
        public string MongoDbDatabaseName { get; set; }
        public string MongoDbConnection { get; set; }

        public int FixedLength { get; set; }

        public string BaseUrl { get; set; }


        public int CacheMaxSize { get; set; }
    }
}
