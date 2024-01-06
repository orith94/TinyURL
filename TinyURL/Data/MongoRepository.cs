using MongoDB.Bson;
using MongoDB.Driver;
using System.Text;
using TinyURL.Interfaces;

namespace TinyURL.Data
{
    public class MongoRepository : IMongoRepository
    {

        private readonly IMongoDatabase _database;

        public MongoRepository(string connectionString, string databaseName)
        {
            var client = new MongoClient(connectionString);
            _database = client.GetDatabase(databaseName);
        }

        public async Task<UrlMapping> GetShortUrl(string originalUrl)
        {
            var mongoCollection = _database.GetCollection<UrlMapping>("UrlMappings");
            var all = await mongoCollection.FindAsync(mapping => mapping.OriginalUrl == originalUrl);

            var shortUrl = await all.FirstOrDefaultAsync();

            return shortUrl;
        }

        public async Task InsertMappingUrl(UrlMapping urlMapping)
        {
            var mongoCollection = _database.GetCollection<UrlMapping>("UrlMappings");
            await mongoCollection.InsertOneAsync(urlMapping);
        }


        public string ShortenURL(int size)
        {
            var id = ObjectId.GenerateNewId();
            // Encode the ObjectId using Base62 encoding
            string base62Encoded = Base62Encode(id);
            string shortCode = base62Encoded.Substring(0, size);

            return shortCode;

        }

        public async Task<UrlMapping> GetOriginalUrl(string ShortUrl)
        {
            var mongoCollection = _database.GetCollection<UrlMapping>("UrlMappings");
            var all = await mongoCollection.FindAsync(mapping => mapping.ShortUrl == ShortUrl);

            var shortUrl = await all.FirstOrDefaultAsync();

            return shortUrl;
        }

        private string Base62Encode(ObjectId objectId)
        {
            var characters = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            var bytes = objectId.ToByteArray();

            StringBuilder result = new StringBuilder();
            var value = BitConverter.ToUInt64(bytes, 0);

            while (value > 0)
            {
                result.Insert(0, characters[(int)(value % 62)]);
                value /= 62;
            }

            return result.ToString();
        }
    }
}
