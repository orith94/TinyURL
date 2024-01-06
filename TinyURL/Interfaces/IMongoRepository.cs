using TinyURL.Data;

namespace TinyURL.Interfaces
{
    public interface IMongoRepository
    {
        string ShortenURL(int size);

        Task<UrlMapping> GetShortUrl(string originalUrl);

        Task InsertMappingUrl(UrlMapping urlMapping);

        Task<UrlMapping> GetOriginalUrl(string shortUrl);
    }
}
