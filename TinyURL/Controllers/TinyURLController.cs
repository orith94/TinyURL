using Microsoft.AspNetCore.Mvc;
using TinyURL.Data;
using TinyURL.Interfaces;
using TinyURL.Utils;

namespace TinyURL.Controllers
{
    [ApiController]
    [Route("api/url")]
    public class TinyURLController : Controller
    {

        private readonly IMongoRepository _mongo;
        private readonly IUrlMappingCache _urlMappingCache;
        private readonly TinyURLConfig _config;

        public TinyURLController(IMongoRepository mongo, IUrlMappingCache urlMappingCache, TinyURLConfig config)
        {
            _mongo = mongo;
            _urlMappingCache = urlMappingCache;
            _config = config;
        }

        [HttpPost]
        public async Task<IActionResult> ShortenUrl([FromBody] string originalUrl)
        {

            var mapper = await _mongo.GetShortUrl(originalUrl);
            if (mapper != null)
            {
                return Ok(mapper.ShortUrl);
            }

            // if the long url has not been shortened
            if (mapper == null)
            {
                var shortCode = _mongo.ShortenURL(_config.FixedLength);
                var shortUrl = $"{_config.BaseUrl}?u={shortCode}";

                // save the mapping in the database
                var urlMapping = new UrlMapping { OriginalUrl = originalUrl, ShortUrl = shortUrl };
                await _mongo.InsertMappingUrl(urlMapping);

                return Ok(shortUrl);


            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> RedirectToOriginalUrl(string shortUrl)
        {
            //check if in cache
            string originalUrl;
            if (_urlMappingCache.TryGet(shortUrl, out originalUrl))
            {
                return Redirect(originalUrl);
            }
            //get from mongo
            var urlMapping = await _mongo.GetOriginalUrl(shortUrl);
            //update cache
            _urlMappingCache.AddOrUpdate(shortUrl, urlMapping.OriginalUrl);

            if (urlMapping == null)
            {
                return NotFound();
            }

            return Redirect(urlMapping.OriginalUrl);
        }
    }
}
